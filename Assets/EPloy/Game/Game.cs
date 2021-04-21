﻿using UnityEngine;
using System;
using EPloy.Res;

namespace EPloy
{
    public class Game : MonoBehaviour
    {
        public static Game Instance = null;
        [SerializeField]
        private bool IsILRuntime;
        [SerializeField]
        private bool EditorResource;

        public static ResUpdaterModule ResUpdater
        {
            get;
            private set;
        }

        public static VersionCheckerModule VersionChecker
        {
            get;
            private set;
        }

        public static FileSystemModule FileSystem
        {
            get;
            private set;
        }

        public static ILRuntimeModule ILRuntime
        {
            get;
            private set;
        }

        public static ProcedureModule Procedure
        {
            get;
            private set;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            FileSystem = EPloyModuleMgr.CreateModule<FileSystemModule>();
            VersionChecker = EPloyModuleMgr.CreateModule<VersionCheckerModule>();
            ResUpdater = EPloyModuleMgr.CreateModule<ResUpdaterModule>();
            ILRuntime = EPloyModuleMgr.CreateModule<ILRuntimeModule>();
            Procedure = EPloyModuleMgr.CreateModule<ProcedureModule>();



            Procedure.StartGame(IsILRuntime, EditorResource);
        }

        private void Update()
        {
            EPloyModuleMgr.ModuleUpdate();
        }

        private void OnDestroy()
        {
            EPloyModuleMgr.ModuleDestory();
        }
    }
}