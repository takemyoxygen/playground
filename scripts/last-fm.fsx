#r @"packages\FSharp.Data\lib\net40\FSharp.Data.dll"
#r @"packages\Newtonsoft.Json\lib\net45\Newtonsoft.Json.dll"
#load @"packages\FSharp.Charting\FSharp.Charting.fsx"

open System
open System.IO
open System.Collections.Generic
open FSharp.Data
open Newtonsoft.Json
open FSharp.Charting
open System.Threading

let apiKey = "--api-key--"
let endpoint = "http://ws.audioscrobbler.com/2.0/"
let home = __SOURCE_DIRECTORY__
fsi.ShowDeclarationValues <- false

let prettify (json: string) = 
    let deserialized = JsonConvert.DeserializeObject(json)
    JsonConvert.SerializeObject(deserialized, Formatting.Indented)

let call methodName args =
  let args = ("method", methodName) :: ("api_key", apiKey) :: ("format", "json") :: args
  let response = Http.Request(endpoint, query = args).Body
  match response with
  | Text(text) -> text |> prettify
  | Binary(_) -> failwithf "Binary response detected when calling method \"%s\"" methodName

type Tracks = JsonProvider<"example.json">
type Tags = JsonProvider<"example-tags.json">

let download page = 
    call "user.getRecentTracks" ["user", "killedthedream"; "limit", "200"; "page", page.ToString()] 
    |> Tracks.Parse

let downloadAllTracks =
    let rec loop page = async {
        let chunk = download page
        printfn "Downloaded page %i / %i" page chunk.Recenttracks.Attr.TotalPages
        if chunk.Recenttracks.Attr.TotalPages > page then
            do! Async.Sleep 200
            let! rest = loop (page + 1)
            return chunk :: rest
        else 
            return [chunk]
    }
    loop 1

let month (date: DateTime) = DateTime(date.Year, date.Month, 1)

let getTagsFor (tracks: Tracks.Track list) = 

    let artistTags artist = 
        let tags =
            call "artist.getTopTags" ["artist", artist; "autocorrect", "1"]
            |> Tags.Parse

        tags.Toptags.Tag
        |> Seq.take (min tags.Toptags.Tag.Length 3)
        |> Seq.map (fun tag -> tag.Name.JsonValue.AsString())
        |> List.ofSeq

    let artists = 
        tracks
        |> Seq.map (fun t -> t.Artist.Text)
        |> Seq.distinct
        |> Array.ofSeq

    artists
    |> Seq.mapi (fun i artist -> 
        printfn "Fetching tags for %s (%i / %i)" artist i artists.Length
        let tags = artistTags artist
        Thread.Sleep 200
        artist, tags)
    |> Map.ofSeq

let loadTracks() =
    let file = Path.Combine(home, "all-scrobbled.json")

    let start = new DateTime(2008, 01, 01)
    let fromFile = File.ReadAllText(file) |> Tracks.Parse

    fromFile.Recenttracks.Track
    |> Seq.filter (fun track -> track.Date.IsSome && track.Date.Value.Text > start)
    |> List.ofSeq

let tracksAndArtistsStatistics (tracks: Tracks.Track list) =
    let tracksPerMonth = 
        tracks
        |> Seq.groupBy (fun track -> month track.Date.Value.Text)
        |> Seq.map (fun (date, tracks) -> (date, tracks |> Seq.length))

    let tracksPerMonthChart = Chart.Line(tracksPerMonth, Title = "Tracks per month")

    let artistsPerMonth =
        tracks
        |> Seq.map (fun track -> new DateTime(track.Date.Value.Text.Year, track.Date.Value.Text.Month, 1), track.Artist.Text)
        |> Seq.groupBy fst
        |> Seq.map (fun (month, datedArtists) -> month, datedArtists |> Seq.map snd |> Seq.distinct)

    let artistsVariosity = 
        artistsPerMonth
        |> Seq.map (fun (month, artists) -> month, Seq.length artists)

    let artistsPerMonthChart = Chart.Line(artistsVariosity, Title = "Artists per month")

    tracksPerMonthChart, artistsPerMonthChart

let normalize (tag: string) =
    tag.ToLower().Replace(" ", "-")

let loadTags () =
    try 
        let text = File.ReadAllText(Path.Combine(home, "tags.json"))
        JsonConvert.DeserializeObject<Map<string, string list>>(text)
        |> Map.map (fun _ tags -> tags |> List.map normalize)
    with
    | e -> 
        printfn "Failed to deserialize json with tags: %O" e
        Map.empty 

let tagsStatistics (tracks: Tracks.Track list) (tagsByArtist: Map<string, string list>) =
    let artistScrobblesPerMonth =
        tracks
        |> Seq.groupBy (fun t -> t.Artist.Text, month t.Date.Value.Text)
        |> Seq.map (fun ((artist, month), tracks) -> artist, month, Seq.length tracks)
        |> List.ofSeq

    let totalScrobblesPerMonth =
        tracks
        |> Seq.groupBy (fun t -> month t.Date.Value.Text)
        |> Seq.map (fun (month, tracks) -> month, tracks |> Seq.length)
        |> Map.ofSeq

    let tagScrobblesPerMonth =
        artistScrobblesPerMonth
        |> Seq.collect (fun (artist, month, count) -> tagsByArtist.[artist] |> Seq.map (fun tag -> tag, month, count))
        |> Seq.groupBy (fun (tag, month, _) -> tag, month)
        |> Seq.map (fun ((tag, month), counts) -> tag, month, counts |> Seq.sumBy (fun (_, _, count) -> count))
        |> List.ofSeq

    let scrobblesPerTag = 
        tagScrobblesPerMonth
        |> Seq.groupBy (fun (tag, _, _) -> tag)
        |> Seq.map (fun (tag, scrobbles) -> tag, scrobbles |> Seq.sumBy (fun (_, _, count) -> count))
        |> List.ofSeq

    let tagsToRender = 
        scrobblesPerTag
        |> Seq.sortBy (fun (_, count) -> -count)
        |> Seq.take 5
        |> Seq.map fst
        |> List.ofSeq

    let tagsRatioPerMonth = 
        tagScrobblesPerMonth
        |> List.map (fun (tag, month, count) -> tag, month, float count / float totalScrobblesPerMonth.[month])



    tagsRatioPerMonth
    |> Seq.filter (fun (tag, _, _) -> tagsToRender |> List.contains tag)
    |> Seq.groupBy (fun (tag, _, _) -> tag)
    |> Seq.map (fun (tag, scrobbles) -> tag, scrobbles |> Seq.map (fun (_, month, count) -> month, count))
    |> Seq.map (fun (tag, scrobbles) -> Chart.Spline(scrobbles, Name = tag))
    |> Chart.Combine
    |> Chart.WithLegend(Enabled = true)
    |> Chart.WithTitle(Text = "Tags ratio per month", InsideArea = false)

let tracks = loadTracks()
let tagsByArtist = loadTags()

let tracksPerMonth, artistsPerMonth = tracksAndArtistsStatistics tracks
let tagsStats = tagsStatistics tracks tagsByArtist

[tracksPerMonth, artistsPerMonth, tagsStats]



