using ETModel;
using GameFramework;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    public static class FsmComponentSystem
    {
        private static HotfixFsmComponent fsm = null;
        private static HotfixFsmComponent Fsm
        {
            get
            {
                if (fsm == null) fsm = GameEntry.Extension.GetComponent<HotfixFsmComponent>();
                return Fsm;
            }
        }

        /// <summary>
        /// 获取有限状态机数量。
        /// </summary>
        public static int HotfixCount(this FsmComponent self)
        {
            return Fsm.Count;
        }

        /// <summary>
        /// 关闭并清理有限状态机管理器。
        /// </summary>
        public static void HotfixShutdown(this FsmComponent self)
        {
            Fsm.Shutdown();
        }

        /// <summary>
        /// 检查是否存在有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <returns>是否存在有限状态机。</returns>
        public static bool HotfixHasFsm(this FsmComponent self, Type ownerType)
        {
            return Fsm.InternalHasFsm(Utility.Text.GetFullName(ownerType, string.Empty));
        }

        /// <summary>
        /// 检查是否存在有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <param name="name">有限状态机名称。</param>
        /// <returns>是否存在有限状态机。</returns>
        public static bool HotfixHasFsm(this FsmComponent self, Type ownerType, string name)
        {
            return Fsm.InternalHasFsm(Utility.Text.GetFullName(ownerType, name));
        }

        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <returns>要获取的有限状态机。</returns>
        public static FsmBase HotfixGetFsm(this FsmComponent self, Type ownerType)
        {
            return Fsm.InternalGetFsm(Utility.Text.GetFullName(ownerType, string.Empty));
        }
        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <param name="name">有限状态机名称。</param>
        /// <returns>要获取的有限状态机。</returns>
        public static FsmBase HotfixGetFsm(this FsmComponent self, Type ownerType, string name)
        {
            return Fsm.InternalGetFsm(Utility.Text.GetFullName(ownerType, name));
        }

        /// <summary>
        /// 获取所有有限状态机。
        /// </summary>
        /// <returns>所有有限状态机。</returns>
        public static FsmBase[] HotfixGetAllFsms(this FsmComponent self)
        {
            return Fsm.GetAllFsms();
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public static IFsm HotfixCreateFsm(this FsmComponent self, object owner, params FsmState[] states)
        {
            return Fsm.CreateFsm(string.Empty, owner, states);
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public static IFsm HotfixCreateFsm(this FsmComponent self, string name, object owner, params FsmState[] states)
        {
            return Fsm.CreateFsm(name, owner, states);
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public static bool HotfixDestroyFsm(this FsmComponent self, Type ownerType)
        {
            return Fsm.InternalDestroyFsm(Utility.Text.GetFullName(ownerType, string.Empty));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <param name="name">要销毁的有限状态机名称。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public static bool HotfixDestroyFsm(this FsmComponent self, Type ownerType, string name)
        {
            return Fsm.InternalDestroyFsm(Utility.Text.GetFullName(ownerType, name));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="fsm">要销毁的有限状态机。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public static bool HotfixDestroyFsm(this FsmComponent self, IFsm fsm)
        {
            return Fsm.InternalDestroyFsm(Utility.Text.GetFullName(fsm.Owner.GetType(), fsm.Name));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <param name="fsm">要销毁的有限状态机。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public static bool HotfixDestroyFsm(this FsmComponent self, FsmBase fsm)
        {
            return Fsm.InternalDestroyFsm(Utility.Text.GetFullName(fsm.OwnerType, fsm.Name));
        }
    }
}