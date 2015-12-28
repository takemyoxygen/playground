// should be called using FsiAnyCPU.exe

open System
open System.IO
open System.Runtime.InteropServices

let assemblyLocation =
  Path.Combine(__SOURCE_DIRECTORY__, "..", "target", "release")
  |> Path.GetFullPath

Environment.SetEnvironmentVariable("Path",
    Environment.GetEnvironmentVariable("Path") + ";" + assemblyLocation)

module Rust =
  [<DllImport("embed.dll", EntryPoint = "process")>]
  extern unit Process();

printfn "Calling Rust..."
Rust.Process()
printfn "Calling Rust complete"
