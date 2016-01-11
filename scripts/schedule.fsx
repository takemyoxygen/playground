open System

type Schedule =
| Daily
| Weekly of DayOfWeek
| Monthly of day: int

let daysFrom (start: DateTime) = 
    Seq.unfold
        (fun (d: DateTime) -> 
            let next = d.AddDays(1.0)
            Some(next, next))
        start

let filterBySchedule schedule (days: DateTime seq) =
    match schedule with
    | Daily -> days
    | Weekly(dayOfWeek) -> days |> Seq.filter (fun d -> d.DayOfWeek = dayOfWeek)
    | Monthly(day) -> 
        days
        |> Seq.filter 
            (fun d -> 
                let totalDays = DateTime.DaysInMonth(d.Year, d.Month)
                let targetDay = if totalDays < day then totalDays else day
                d.Day = targetDay)

let nth n xs = seq {
    let mutable skipped = 0
    for x in xs do
        if skipped = n - 1
        then 
            skipped <- 0
            yield x
        else
            skipped <- skipped + 1
}

let today = DateTime.Today;
let mondays = Weekly(DayOfWeek.Monday)
let lastDays = Monthly(31)

daysFrom today
|> filterBySchedule lastDays
|> Seq.take 30
|> Seq.map (fun d -> sprintf "%O %s" d.DayOfWeek (d.ToString("dd-MM-yyyy")))
|> Seq.iter (printfn "%s")
