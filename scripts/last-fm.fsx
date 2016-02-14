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


type ScrobbledTrack =
    { Artist: string;
      Track: string;
      Date: DateTime }

type Tags = Map<string, string list>

let apiKey = "--api-key--"
let endpoint = "http://ws.audioscrobbler.com/2.0/"
let home = __SOURCE_DIRECTORY__
fsi.ShowDeclarationValues <- false

module Data =

    module LastFm =

        type private Tags = JsonProvider<"example-tags.json">

        let private prettify (json: string) =
            let deserialized = JsonConvert.DeserializeObject json
            JsonConvert.SerializeObject(deserialized, Formatting.Indented)

        let call methodName args =
            let args = ("method", methodName) :: ("api_key", apiKey) :: ("format", "json") :: args
            let response = Http.Request(endpoint, query = args).Body
            match response with
            | Text(text) -> text |> prettify
            | Binary(_) -> failwithf "Binary response detected when calling method \"%s\"" methodName

        let getTagsFor artists =

            let artistTags artist =
                let tags =
                    call "artist.getTopTags" ["artist", artist; "autocorrect", "1"]
                    |> Tags.Parse

                match tags.JsonValue.TryGetProperty("error"), tags.JsonValue.TryGetProperty("message") with
                | Some(_), Some(msg) ->
                    printfn "An error ocurred while fetching tags for \"%s\": %O" artist msg
                    []
                | _ ->
                    tags.Toptags.Tag
                    |> Seq.take (min tags.Toptags.Tag.Length 3)
                    |> Seq.map (fun tag -> tag.Name.JsonValue.AsString())
                    |> List.ofSeq

            artists
            |> Seq.mapi (fun i artist ->
                printfn "Fetching tags for %s (%i / %i)" artist i (List.length artists)
                let tags = artistTags artist
                Thread.Sleep 200
                artist, tags)
            |> Map.ofSeq

    type private Scrobbles = CsvProvider<"last-fm-tracks.csv", HasHeaders = false, Schema = "Artist, Album, Track, Date (date option)">

    let private start = DateTime(2008, 1, 1)

    let artistsOf tracks =
        tracks
        |> Seq.map (fun t -> t.Artist)
        |> Seq.distinct
        |> List.ofSeq

    let loadScrobbledTracks() =
        Scrobbles.GetSample().Rows
        |> Seq.filter (fun row -> row.Date.IsSome)
        |> Seq.filter (fun row -> row.Date.Value > start)
        |> Seq.map (fun row -> { Artist = row.Artist; Track = row.Track; Date = row.Date.Value })
        |> List.ofSeq

    let private normalize (tag: string) = tag.ToLower().Replace(" ", "-")

    let private loadTagsFrom file =
        try
            let text = File.ReadAllText(Path.Combine(home, file))
            JsonConvert.DeserializeObject<Tags>(text)
            |> Map.map (fun _ tags -> tags |> List.map normalize)
        with
        | e ->
            printfn "Failed to deserialize json with tags: %O" e
            Map.empty

    let private saveTags tags file =
        let json = JsonConvert.SerializeObject(tags)
        File.WriteAllText(Path.Combine(home, file), json)

    let getTagsFor artists =
        let cacheFile = "tags.json"
        let cached = loadTagsFrom cacheFile
        let missing = artists |> List.filter (cached.ContainsKey >> not)
        match missing with
        | [] -> cached
        | _ ->
            printfn "Loading tags from Last.fm for %A" missing
            let newTags = LastFm.getTagsFor missing
            let tags =
                newTags
                |> Map.toSeq
                |> Seq.append (cached |> Map.toSeq)
                |> Map.ofSeq

            saveTags tags cacheFile
            tags

module Analysis =

    let month (date: DateTime) = DateTime(date.Year, date.Month, 1)

    let tracksAndArtistsStatistics (tracks: ScrobbledTrack list) =
        let tracksPerMonth =
            tracks
            |> Seq.groupBy (fun track -> month track.Date)
            |> Seq.map (fun (date, tracks) -> (date, tracks |> Seq.length))

        let tracksPerMonthChart = Chart.Line(tracksPerMonth, Title = "Tracks per month")

        let artistsPerMonth =
            tracks
            |> Seq.map (fun track -> new DateTime(track.Date.Year, track.Date.Month, 1), track.Artist)
            |> Seq.groupBy fst
            |> Seq.map (fun (month, datedArtists) -> month, datedArtists |> Seq.map snd |> Seq.distinct)

        let artistsVariosity =
            artistsPerMonth
            |> Seq.map (fun (month, artists) -> month, Seq.length artists)

        let artistsPerMonthChart = Chart.Line(artistsVariosity, Title = "Artists per month")

        tracksPerMonthChart, artistsPerMonthChart

    let tagsStatistics (tracks: ScrobbledTrack list) (tagsByArtist: Map<string, string list>) =
        let artistScrobblesPerMonth =
            tracks
            |> Seq.groupBy (fun t -> t.Artist, month t.Date)
            |> Seq.map (fun ((artist, month), tracks) -> artist, month, Seq.length tracks)
            |> List.ofSeq

        let totalScrobblesPerMonth =
            tracks
            |> Seq.groupBy (fun t -> month t.Date)
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

let tracks = Data.loadScrobbledTracks()
let artists = Data.artistsOf tracks
let tagsByArtist = Data.getTagsFor artists

let tracksPerMonth, artistsPerMonth = Analysis.tracksAndArtistsStatistics tracks
let tagsStats = Analysis.tagsStatistics tracks tagsByArtist

[tracksPerMonth, artistsPerMonth, tagsStats]
