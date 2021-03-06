/*
 * FPLedit Release-Prozess
 * Erstellt aus einem Ordner mit Kompilaten eine ZIP-Datei
 * Aufruf: build-release.csx $(TargetDir)
 * Version 0.7 / (c) Manuel Huber 2020
 */

#r "System.IO.Compression.FileSystem.dll"
#load "includes.csx"

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.IO.Compression;
using System.Collections.Generic;

Console.WriteLine("Post-Build: Erstelle Binärpaket!");

var output_path = Path.GetFullPath(Args[0]);
var version = GetProductVersion(output_path);

/*
 * TASK: Check
 */
DirectoryInfo info = new DirectoryInfo(output_path);
var required_files = new [] {
    "FPLedit.Shared.UI.PlatformControls.Gtk.dll",
    "FPLedit.Shared.UI.PlatformControls.Wpf.dll",
};

var had_error = false;
foreach (var file in required_files) {
    if (info.GetFiles(file).Length == 0)
    {
        Error($"Binärpaket kann nicht ohne erforderliche Komponente {file} gebaut werden!");
        had_error = true;
    }
}

if (had_error)
    Environment.Exit(-1);

/*
 * TASK: Cleanup release files
 */

Console.WriteLine("Entferne PDBs");
var files = info.GetFiles("*.pdb");
foreach (var f in files)
{
    File.Delete(f.FullName);
    Console.WriteLine(f.Name);
}

Console.WriteLine("Entferne *.deps.json");
files = info.GetFiles("*.deps.json");
foreach (var f in files)
{
    File.Delete(f.FullName);
    Console.WriteLine(f.Name);
}

Console.WriteLine("Entferne Installer!");
files = info.GetFiles("FPLedit.Installer.exe");
foreach (var f in files)
{
    File.Delete(f.FullName);
    Console.WriteLine(f.Name);
}

/*
 * TASK: Move library files
 */
Console.WriteLine("Verschiebe Bibliotheken!");

var dll_files = info.GetFiles("*.dll").Select(f => f.Name);
var fpledit_files = info.GetFiles("FPLedit.*").Select(f => f.Name);
var dll_fns_move = dll_files.Except(fpledit_files);
var eto_dest = info.CreateSubdirectory("lib");
foreach (var f in dll_fns_move)
{
    var src_fn = Path.Combine(info.FullName, f);
    var dst_fn = Path.Combine(eto_dest.FullName, f);
    if (File.Exists(dst_fn)) // Datei existiert bereits
        File.Delete(dst_fn);
    File.Move(src_fn, dst_fn);
    Console.WriteLine(f);
}

Console.WriteLine();

/*
 * TASK: Build new license file
 */
Console.WriteLine("Generiere neue README-Datei");
var license = GetLicense(version);
var license_path = Path.Combine(output_path, "README_LICENSE.txt");
File.WriteAllText(license_path, license);

/*
 * TASK: Add offline documentation file
 */
var doc = Environment.GetEnvironmentVariable("FPLEDIT_DOK");
var doc_generated = false;
if (doc != null && File.Exists(doc))
{
    Console.WriteLine("Kopiere Offline-Dokumentation");
    var doc_path = Path.Combine(output_path, "doku.html");
    if (File.Exists(doc_path))
        File.Delete(doc_path);
    File.Copy(doc, doc_path);
    doc_generated = true;
}
else
    Warning("Umgebungsvariable FPLEDIT_DOK nicht gesetzt bzw. die Datei (= Wert der Variablen) existiert nicht! Das generierte Programmpaket enthält keine Dokumentation!");

/*
 * TASK: Build ZIP file
 */
Console.WriteLine("Erstelle ZIP-Datei");
var result_path = Path.Combine(output_path, "..", $"fpledit-{version}{(doc_generated ? "" : "-nodoc")}.zip");

if (File.Exists(result_path))
    Warning($"ZIP-Datei {result_path} existiert bereits und wurde nicht erneut generiert!");
else
{
    ZipFile.CreateFromDirectory(output_path, result_path);
    Console.WriteLine("ZIP-Datei {0} erstellt!\n", result_path);
}

Console.WriteLine("Post-Build erfolgreich abgeschlossen!");


private class FileInfoComparer : IEqualityComparer<FileInfo>
{
    public bool Equals(FileInfo x, FileInfo y) => x == null ? y == null : (x.Name.Equals(y.Name, StringComparison.CurrentCultureIgnoreCase) && x.Length == y.Length);

    public int GetHashCode(FileInfo obj) => obj.GetHashCode();
}
