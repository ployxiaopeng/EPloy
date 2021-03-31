using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3> s_UnityEngine_Vector3_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion> s_UnityEngine_Quaternion_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2> s_UnityEngine_Vector2_Binding_Binder = null;

        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            ETModel_DataTableComponent_Binding.Register(app);
            System_Char_Binding.Register(app);
            System_String_Binding.Register(app);
            System_Int32_Binding.Register(app);
            System_Single_Binding.Register(app);
            GameFramework_DataTable_IDataTable_1_ETModel_IDataRowBaseAdaptor_Binding_Adaptor_Binding.Register(app);
            System_Text_RegularExpressions_Regex_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            ETModel_Log_Binding.Register(app);
            System_Text_StringBuilder_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_Collections_IEnumerable_Binding.Register(app);
            System_Collections_IEnumerator_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Type_Binding.Register(app);
            Google_Protobuf_ByteString_Binding.Register(app);
            ETModel_ByteHelper_Binding.Register(app);
            System_Array_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            System_Reflection_PropertyInfo_Binding.Register(app);
            System_Reflection_MethodBase_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            System_Security_Cryptography_MD5CryptoServiceProvider_Binding.Register(app);
            System_Text_Encoding_Binding.Register(app);
            System_Security_Cryptography_HashAlgorithm_Binding.Register(app);
            System_Convert_Binding.Register(app);
            System_Security_Cryptography_SHA1CryptoServiceProvider_Binding.Register(app);
            System_Byte_Binding.Register(app);
            System_Exception_Binding.Register(app);
            System_Collections_IDictionary_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_GameObject_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Sprite_Binding.Register(app);
            System_Collections_Generic_Queue_1_String_Binding.Register(app);
            System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Binding.Register(app);
            GameFramework_Utility_Binding_Text_Binding.Register(app);
            UnityEngine_UI_Image_Binding.Register(app);
            ETModel_DataRowBase_Binding.Register(app);
            UnityEngine_SpriteRenderer_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_String_Binding.Register(app);
            ETModel_ResourceComponentSystem_Binding.Register(app);
            System_Threading_Tasks_Task_1_GameObject_Binding.Register(app);
            System_Runtime_CompilerServices_TaskAwaiter_1_GameObject_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            ETModel_Altas_Binding.Register(app);
            System_Collections_Generic_List_1_Sprite_Binding.Register(app);
            System_Collections_Generic_List_1_Sprite_Binding_Enumerator_Binding.Register(app);
            ETModel_AssetUtility_Binding.Register(app);
            ETModel_ConfigComponent_Binding.Register(app);
            ETModel_AsyncETVoidMethodBuilder_Binding.Register(app);
            ETModel_EntityComponent_Binding.Register(app);
            ETModel_HotfixEntityEvent_Binding.Register(app);
            ETModel_EventComponent_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Object_Binding.Register(app);
            ETModel_EntityLogic_Binding.Register(app);
            ETModel_EntityModelLogic_Binding.Register(app);
            GameFramework_Utility_Binding_Assembly_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int32_ILTypeInstance_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_Quaternion_Binding.Register(app);
            GameFramework_Entity_IEntityGroup_Binding.Register(app);
            ETModel_EntityGroupHelper_Binding.Register(app);
            ETModel_ILRuntimeComponent_Binding.Register(app);
            System_Collections_Generic_List_1_Type_Binding.Register(app);
            System_Collections_Generic_List_1_Type_Binding_Enumerator_Binding.Register(app);
            System_Activator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Type_ILTypeInstance_Binding.Register(app);
            ETModel_HotfixUpdateEvent_Binding.Register(app);
            ETModel_HotfixLateUpdateEvent_Binding.Register(app);
            GameFramework_ReferencePool_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            GameFramework_GameFrameworkException_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Variable_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Dictionary_2_Int32_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int32_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Vector2_ETModel_IDataRowBaseAdaptor_Binding_Adaptor_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Vector2_Boolean_Binding.Register(app);
            System_Collections_Generic_Queue_1_ILTypeInstance_Binding.Register(app);
            UnityEngine_Mathf_Binding.Register(app);
            UnityEngine_Random_Binding.Register(app);
            DG_Tweening_ShortcutExtensions_Binding.Register(app);
            UnityEngine_WaitForFixedUpdate_Binding.Register(app);
            System_Collections_Generic_List_1_Vector2_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);
            UnityEngine_Animator_Binding.Register(app);
            ETModel_ModelGame_Binding.Register(app);
            ETModel_Entity_Binding.Register(app);
            ETModel_OpcodeTypeComponent_Binding.Register(app);
            ETModel_MessageAttribute_Binding.Register(app);
            ETModel_DoubleMap_2_UInt16_Type_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_UInt16_Object_Binding.Register(app);
            ETModel_IMHandler_Binding.Register(app);
            ETModel_MessageDispatcherComponent_Binding.Register(app);
            ETModel_SceneComponent_Binding.Register(app);
            GameFramework_Scene_ISceneManager_Binding.Register(app);
            GameFramework_Fsm_IFsm_1_IProcedureManager_Binding.Register(app);
            GameFramework_Variable_1_String_Binding.Register(app);
            UnityEngine_SceneManagement_SceneManager_Binding.Register(app);
            UnityEngine_SceneManagement_Scene_Binding.Register(app);
            GameFramework_Scene_LoadSceneSuccessEventArgs_Binding.Register(app);
            GameFramework_Scene_LoadSceneFailureEventArgs_Binding.Register(app);
            GameFramework_Scene_LoadSceneUpdateEventArgs_Binding.Register(app);
            GameFramework_Scene_LoadSceneDependencyAssetEventArgs_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Boolean_Binding.Register(app);
            ETModel_ETVoid_Binding.Register(app);
            GameFramework_Procedure_ProcedureBase_Binding.Register(app);
            ETModel_ProcedureComponent_Binding.Register(app);
            ETModel_FsmComponent_Binding.Register(app);
            GameFramework_Procedure_IProcedureManager_Binding.Register(app);
            ETModel_TimerComponent_Binding.Register(app);
            ETModel_ETTask_Binding.Register(app);
            ETModel_ETTask_Binding_Awaiter_Binding.Register(app);
            ETModel_VarString_Binding.Register(app);
            GameFramework_Config_IConfigManager_Binding.Register(app);
            GameFramework_DataTable_IDataTableManager_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Boolean_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_Boolean_Binding.Register(app);
            GameFramework_Config_LoadConfigSuccessEventArgs_Binding.Register(app);
            ETModel_LoadConfigInfo_Binding.Register(app);
            GameFramework_Config_LoadConfigFailureEventArgs_Binding.Register(app);
            GameFramework_DataTable_LoadDataTableSuccessEventArgs_Binding.Register(app);
            ETModel_LoadDataTableInfo_Binding.Register(app);
            GameFramework_DataTable_LoadDataTableFailureEventArgs_Binding.Register(app);
            GameFramework_Resource_LoadAssetCallbacks_Binding.Register(app);
            System_Collections_Generic_ICollection_1_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_IEnumerable_1_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_IDictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_Text_Binding.Register(app);
            ETModel_UIForm_Binding.Register(app);
            GameFramework_UI_IUIGroup_Binding.Register(app);
            ETModel_UIGroupHelper_Binding.Register(app);
            ETModel_UIComponent_Binding.Register(app);
            System_Collections_Generic_List_1_Text_Binding_Enumerator_Binding.Register(app);
            ETModel_ResourceComponent_Binding.Register(app);
            ETModel_UIGroup_Binding.Register(app);
            ETModel_HotfixUIEvent_Binding.Register(app);
            ETModel_UIFormLogic_Binding.Register(app);
            ETModel_ShowUIEvent_1_Int32_Binding.Register(app);
            GameFramework_BaseEventArgs_Binding.Register(app);
            System_IO_BinaryReader_Binding.Register(app);
            UnityEngine_Color32_Binding.Register(app);
            UnityEngine_Color_Binding.Register(app);
            System_DateTime_Binding.Register(app);
            UnityEngine_Rect_Binding.Register(app);
            UnityEngine_Vector4_Binding.Register(app);
            System_Collections_Generic_List_1_String_Binding.Register(app);
            System_Collections_Generic_List_1_Int32_Binding.Register(app);
            System_Collections_Generic_List_1_Single_Binding.Register(app);
            System_BitConverter_Binding.Register(app);
            GameFramework_GameFrameworkSegment_1_Byte_Array_Binding.Register(app);
            System_IO_MemoryStream_Binding.Register(app);
            UnityEngine_LayerMask_Binding.Register(app);
            ETModel_Init_Binding.Register(app);
            GameFramework_Fsm_IFsmManager_Binding.Register(app);
            ETModel_OneThreadSynchronizationContext_Binding.Register(app);
            System_Threading_SynchronizationContext_Binding.Register(app);
            UnityEngine_CanvasGroup_Binding.Register(app);
            DG_Tweening_ShortcutExtensions46_Binding.Register(app);
            DG_Tweening_TweenSettingsExtensions_Binding.Register(app);
            ETModel_UIEventListener_Binding.Register(app);
            System_Collections_Generic_List_1_Boolean_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_LinkedList_1_EventHandler_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_LinkedList_1_EventHandler_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_LinkedListNode_1_EventHandler_1_ILTypeInstance_Binding.Register(app);
            System_EventHandler_1_ILTypeInstance_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector3));
            s_UnityEngine_Vector3_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Quaternion));
            s_UnityEngine_Quaternion_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector2));
            s_UnityEngine_Vector2_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2>;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            s_UnityEngine_Vector3_Binding_Binder = null;
            s_UnityEngine_Quaternion_Binding_Binder = null;
            s_UnityEngine_Vector2_Binding_Binder = null;
        }
    }
}
