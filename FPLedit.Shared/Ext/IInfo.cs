﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FPLedit.Shared
{
    public interface IInfo
    {
        Timetable Timetable { get; set; }

        FileState FileState { get; set; }

        void SetUnsaved();

        void BackupTimetable();

        void RestoreTimetable();

        void ClearBackup();

        void Register<T>(T obj);

        T[] GetRegistered<T>();

        // FileHandling
        void Open();

        void Save(bool forceSaveAs);

        void Reload();

        string GetTemp(string filename);

        dynamic Menu { get; }

        ILog Logger { get; }

        ISettings Settings { get; }

        event EventHandler<FileStateChangedEventArgs> FileStateChanged;
    }

    public class FileStateChangedEventArgs : EventArgs
    {
        public FileState FileState { get; private set; }

        public FileStateChangedEventArgs(FileState state)
        {
            FileState = state;
        }
    }
}