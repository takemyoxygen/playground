#r "System.Windows.Forms"
#r "System.Drawing"

#I __SOURCE_DIRECTORY__
#r @"packages\Rx-Core\lib\net40\System.Reactive.Core.dll"
#r @"packages\Rx-Linq\lib\net40\System.Reactive.Linq.dll"
#r @"packages\Rx-Interfaces\lib\net40\System.Reactive.Interfaces.dll"
#r @"packages\FSharp.Control.Reactive\lib\net40\FSharp.Control.Reactive.dll"

open System
open System.Drawing
open System.IO
open System.Windows.Forms
open System.Threading

open System.Reactive.Linq
open System.Reactive.Disposables

open FSharp.Control.Reactive

let delay (interval: int) f =
    Thread.Sleep interval
    f() |> ignore

let notify text title () =
    let icon = new NotifyIcon(Visible = true, Icon = SystemIcons.Information)
    icon.ShowBalloonTip(3000, title, text, ToolTipIcon.Info);

let watch file =
    let directory = Path.GetDirectoryName file
    let extension = Path.GetExtension file
    let filter = "*" + extension
    Observable.Create(fun (observer: IObserver<_>) ->
        let watcher = new FileSystemWatcher(directory, filter, EnableRaisingEvents = true)
        let subscription =
            watcher.Changed
            |> Observable.filter (fun args -> args.FullPath = file)
            |> Observable.filter (fun args -> args.ChangeType = WatcherChangeTypes.Changed)
            |> Observable.map ignore
            |> Observable.throttle (TimeSpan.FromMilliseconds 50.0)
            |> Observable.subscribe observer.OnNext
        new CompositeDisposable(watcher, subscription) :> IDisposable)

let changes =
    watch @"C:\Users\takemyoxygen\Documents\code\playground\scripts\text.txt"
    |> Observable.iter (fun _ -> printfn "File changed")
    |> Observable.subscribe (notify "File has been changed" "Alert")
