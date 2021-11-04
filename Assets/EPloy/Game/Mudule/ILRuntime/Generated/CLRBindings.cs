using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

//will auto register in unity
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static private void RegisterBindingAction()
        {
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
        }

        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3> s_UnityEngine_Vector3_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion> s_UnityEngine_Quaternion_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2> s_UnityEngine_Vector2_Binding_Binder = null;

        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            UnityEngine_Object_Binding.Register(app);
            System_String_Binding.Register(app);
            EPloy_Log_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Input_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            UnityEngine_UI_Button_Binding.Register(app);
            UnityEngine_Events_UnityEvent_Binding.Register(app);
            UnityEngine_UI_Image_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            UnityEngine_UI_Graphic_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding.Register(app);
            EPloy_Utility_Binding_Text_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Linq_Enumerable_Binding.Register(app);
            System_Char_Binding.Register(app);
            System_Int32_Binding.Register(app);
            System_Single_Binding.Register(app);
            System_Text_RegularExpressions_Regex_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Type_ILTypeInstance_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Activator_Binding.Register(app);
            System_Threading_Monitor_Binding.Register(app);
            System_Collections_Generic_Queue_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Vector2_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Vector2_Boolean_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            UnityEngine_Mathf_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            UnityEngine_Animator_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Vector2_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Vector2_ILTypeInstance_Binding.Register(app);
            UnityEngine_SpriteRenderer_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_GameObject_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Queue_1_String_Binding.Register(app);
            System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_Threading_Tasks_Task_1_GameObject_Binding.Register(app);
            System_Runtime_CompilerServices_TaskAwaiter_1_GameObject_Binding.Register(app);
            EPloy_Altas_Binding.Register(app);
            System_Collections_Generic_List_1_Sprite_Binding.Register(app);
            System_Collections_Generic_List_1_Sprite_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_TypeNamePair_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_TypeNamePair_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_TypeNamePair_ILTypeInstance_Binding.Register(app);
            EPloy_TypeNamePair_Binding.Register(app);
            System_IO_BinaryReader_Binding.Register(app);
            UnityEngine_Color32_Binding.Register(app);
            UnityEngine_Color_Binding.Register(app);
            System_DateTime_Binding.Register(app);
            UnityEngine_Quaternion_Binding.Register(app);
            UnityEngine_Rect_Binding.Register(app);
            UnityEngine_Vector4_Binding.Register(app);
            System_IO_MemoryStream_Binding.Register(app);
            System_Text_Encoding_Binding.Register(app);
            System_Byte_Binding.Register(app);
            System_Runtime_CompilerServices_AsyncTaskMethodBuilder_1_GameObject_Binding.Register(app);
            System_Threading_Tasks_TaskCompletionSource_1_Object_Binding.Register(app);
            System_Exception_Binding.Register(app);
            EPloy_GameStart_Binding.Register(app);
            EPloy_GameModule_Binding.Register(app);
            EPloy_Res_ResUpdaterModule_Binding.Register(app);
            EPloy_Res_ResInfo_Binding.Register(app);
            EPloy_Res_ResName_Binding.Register(app);
            System_IO_Path_Binding.Register(app);
            EPloy_Utility_Binding_Path_Binding.Register(app);
            UnityEngine_Application_Binding.Register(app);
            EPloy_TypeLinkedList_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_LinkedListNode_1_ILTypeInstance_Binding.Register(app);
            System_TimeSpan_Binding.Register(app);
            UnityEditor_AssetDatabase_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Object_Binding.Register(app);
            UnityEngine_AsyncOperation_Binding.Register(app);
            UnityEditor_EditorUtility_Binding.Register(app);
            UnityEngine_SceneManagement_SceneManager_Binding.Register(app);
            System_IO_File_Binding.Register(app);
            System_IO_FileInfo_Binding.Register(app);
            System_IO_Directory_Binding.Register(app);
            System_StringComparer_Binding.Register(app);
            System_Collections_Generic_List_1_String_Binding.Register(app);
            EPloy_UnOrderMultiMap_2_Int32_ILTypeInstance_Binding.Register(app);
            EPloy_TypeLinkedList_1_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_String_Binding.Register(app);
            System_Enum_Binding.Register(app);
            System_Array_Binding.Register(app);
            System_Collections_IEnumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_String_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int32_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Type_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Boolean_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Boolean_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_Boolean_Binding.Register(app);
            EPloy_TimerModule_Binding.Register(app);
            EPloy_ETTask_Binding.Register(app);
            EPloy_ETTask_Binding_Awaiter_Binding.Register(app);
            UnityEngine_SceneManagement_Scene_Binding.Register(app);
            System_Collections_ICollection_Binding.Register(app);
            System_Collections_Generic_List_1_RectTransform_Binding.Register(app);
            System_Collections_Generic_List_1_RectTransform_Binding_Enumerator_Binding.Register(app);
            UnityEngine_UI_ScrollRect_Binding.Register(app);
            EPloy_UIEventListener_Binding.Register(app);
            UnityEngine_Canvas_Binding.Register(app);
            UnityEngine_RectTransform_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Type_Binding.Register(app);
            EPloy_VirtualList_Binding.Register(app);
            UnityEngine_EventSystems_IBeginDragHandler_Binding.Register(app);
            UnityEngine_EventSystems_IEndDragHandler_Binding.Register(app);
            UnityEngine_EventSystems_IDragHandler_Binding.Register(app);
            EPloy_ILRuntimeModule_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            UnityEngine_AssetBundle_Binding.Register(app);
            UnityEngine_Networking_UnityWebRequest_Binding.Register(app);
            UnityEngine_Networking_DownloadHandler_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);
            System_Collections_Generic_List_1_Object_Binding.Register(app);
            System_Collections_Generic_List_1_Object_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Object_Int32_Binding.Register(app);
            UnityEngine_AssetBundleCreateRequest_Binding.Register(app);
            UnityEngine_AssetBundleRequest_Binding.Register(app);
            System_GC_Binding.Register(app);
            System_Collections_Generic_HashSet_1_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Object_Binding_t1.Register(app);
            System_Collections_Generic_Dictionary_2_Object_Object_Binding.Register(app);
            EPloy_Utility_Binding_Converter_Binding.Register(app);
            EPloy_Utility_Binding_Encryption_Binding.Register(app);
            EPloy_Res_AssetInfo_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Object_ILTypeInstance_Binding.Register(app);
            EPloy_UnOrderMultiMap_2_String_ILTypeInstance_Binding.Register(app);
            EPloy_UnOrderMultiMap_2_String_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_TypeLinkedList_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Object_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Object_ILTypeInstance_Binding.Register(app);
            System_IO_Stream_Binding.Register(app);
            System_Collections_Generic_Stack_1_ILTypeInstance_Binding.Register(app);

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
