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
            System_UInt32_Binding.Register(app);
            System_String_Binding.Register(app);
            Log_Binding.Register(app);
            System_Int32_Binding.Register(app);
            UnityEngine_Input_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            UIEventListener_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            UnityEngine_UI_Image_Binding.Register(app);
            EPloy_Game_GameModule_Binding.Register(app);
            EPloy_Game_TimerModule_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Single_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            DG_Tweening_ShortcutExtensions_Binding.Register(app);
            DG_Tweening_TweenSettingsExtensions_Binding.Register(app);
            UnityEngine_EventSystems_PointerEventData_Binding.Register(app);
            UnityEngine_RectTransformUtility_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            EPloy_Game_IDPack_Binding.Register(app);
            System_Collections_Generic_List_1_String_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            UtilText_Binding.Register(app);
            UnityEngine_UI_Button_Binding.Register(app);
            UnityEngine_Events_UnityEvent_Binding.Register(app);
            System_Object_Binding.Register(app);
            EPloy_Game_AtlasMudule_Binding.Register(app);
            UnityEngine_SpriteRenderer_Binding.Register(app);
            System_Single_Binding.Register(app);
            System_Activator_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding_ValueCollection_Binding_Enumerator_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_String_Binding.Register(app);
            UnityEngine_Animator_Binding.Register(app);
            UnityEngine_AnimatorStateInfo_Binding.Register(app);
            System_Collections_Generic_List_1_Int32_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            UtilVector_Binding.Register(app);
            UnityEngine_Quaternion_Binding.Register(app);
            UnityEngine_CharacterController_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_TypeNamePair_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_TypeNamePair_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_TypeNamePair_ILTypeInstance_Binding.Register(app);
            TypeNamePair_Binding.Register(app);
            EPloy_Game_ResMudule_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_Threading_Monitor_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Type_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Queue_1_ILTypeInstance_Binding.Register(app);
            EPloy_Game_Res_LoadSceneCallbacks_Binding.Register(app);
            EPloy_Game_Res_UnloadSceneCallbacks_Binding.Register(app);
            System_Linq_Enumerable_Binding.Register(app);
            UnOrderMultiMap_2_Int32_ILTypeInstance_Binding.Register(app);
            TypeLinkedList_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_LinkedListNode_1_ILTypeInstance_Binding.Register(app);
            EPloy_Game_ILRuntimeModule_Binding.Register(app);
            EPloy_Game_Net_TcpNetChannel_Binding.Register(app);
            EPloy_Game_Net_NetChannel_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            EPloy_Game_Net_Packet_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Type_Binding.Register(app);
            EPloy_Game_Net_ProtobufHelper_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            System_IO_MemoryStream_Binding.Register(app);
            System_IO_Stream_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_UInt32_ILTypeInstance_Binding.Register(app);
            GameStart_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_UInt32_String_Binding.Register(app);
            EPloy_Game_Res_LoadAssetCallbacks_Binding.Register(app);
            System_Enum_Binding.Register(app);
            System_Array_Binding.Register(app);
            System_Collections_IEnumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int32_ILTypeInstance_Binding.Register(app);
            EPloy_Game_ObjectPoolMudule_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_UInt32_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_UInt32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_UInt32_String_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_UInt32_String_Binding.Register(app);
            EPloy_Game_UtilAsset_Binding.Register(app);
            EPloy_Game_ObjectPool_ObjectBase_Binding.Register(app);
            EPloy_Game_Obj_ObjectInstance_Binding.Register(app);
            EPloy_Game_AtlasLoadData_Binding.Register(app);
            UnityEngine_SceneManagement_SceneManager_Binding.Register(app);
            UnityEngine_SceneManagement_Scene_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Boolean_Binding.Register(app);
            System_Collections_ICollection_Binding.Register(app);
            System_Collections_Generic_List_1_RectTransform_Binding.Register(app);
            System_Collections_Generic_List_1_RectTransform_Binding_Enumerator_Binding.Register(app);
            UnityEngine_UI_ScrollRect_Binding.Register(app);
            System_Action_2_Int32_GameObject_Binding.Register(app);
            EPloy_Game_ObjectPool_ObjectPoolBase_Binding.Register(app);
            UnityEngine_RectTransform_Binding.Register(app);
            UnityEngine_Canvas_Binding.Register(app);
            UnityEngine_Screen_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Int32_Binding.Register(app);
            EPloy_Game_Obj_ObjectUIForm_Binding.Register(app);
            VirtualList_Binding.Register(app);
            UnityEngine_Mathf_Binding.Register(app);
            TypeLinkedList_1_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_Queue_1_Object_Binding.Register(app);
            System_Collections_Generic_Queue_1_Object_Binding.Register(app);
            Google_Protobuf_ProtoPreconditions_Binding.Register(app);
            Google_Protobuf_CodedOutputStream_Binding.Register(app);
            Google_Protobuf_CodedInputStream_Binding.Register(app);
            Google_Protobuf_MessageParser_1_Google_Protobuf_IMessageAdaptor_Binding_Adaptor_Binding.Register(app);
            Google_Protobuf_Collections_RepeatedField_1_Google_Protobuf_IMessageAdaptor_Binding_Adaptor_Binding.Register(app);
            Google_Protobuf_FieldCodec_Binding.Register(app);
            System_IO_BinaryReader_Binding.Register(app);
            UnityEngine_Color32_Binding.Register(app);
            UnityEngine_Color_Binding.Register(app);
            System_DateTime_Binding.Register(app);
            UnityEngine_Rect_Binding.Register(app);
            UnityEngine_Vector4_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            System_Collections_Generic_List_1_Single_Binding.Register(app);
            System_BitConverter_Binding.Register(app);
            System_Byte_Binding.Register(app);
            System_Char_Binding.Register(app);
            System_Text_Encoding_Binding.Register(app);
            EPloy_Game_Res_LoadBinaryCallbacks_Binding.Register(app);
            System_Predicate_1_ILTypeInstance_Binding.Register(app);

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
