﻿namespace EPloy
{

    //
    // 摘要:
    //     封装一个方法，该方法具无参数并且不返回值。
    public delegate void EPloyAction();

    //
    // 摘要:
    //     封装一个方法，该方法具有一个参数并且不返回值。
    //
    // 参数:
    //   arg1:
    //     此委托封装的方法的第一个参数。
    // 类型参数:
    //   T1:
    //     此委托封装的方法的第一个参数的类型。
    public delegate void EPloyAction<in T1>(T1 arg1);

    //
    // 摘要:
    //     封装一个方法，该方法具有二个参数并且不返回值。
    //
    // 参数:
    //   arg1:
    //     此委托封装的方法的第一个参数。
    //
    //   arg2:
    //     此委托封装的方法的第二个参数。
    // 类型参数:
    //   T1:
    //     此委托封装的方法的第一个参数的类型。
    //
    //   T2:
    //     此委托封装的方法的第二个参数的类型。
    public delegate void EPloyAction<in T1, in T2>(T1 arg1, T2 arg2);

    //
    // 摘要:
    //     封装一个方法，该方法具有四个参数并且不返回值。
    //
    // 参数:
    //   arg1:
    //     此委托封装的方法的第一个参数。
    //
    //   arg2:
    //     此委托封装的方法的第二个参数。
    //
    //   arg3:
    //     此委托封装的方法的第三个参数。
    // 类型参数:
    //   T1:
    //     此委托封装的方法的第一个参数的类型。
    //
    //   T2:
    //     此委托封装的方法的第二个参数的类型。
    //
    //   T3:
    //     此委托封装的方法的第三个参数的类型。
    public delegate void EPloyAction<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);

    //
    // 摘要:
    //     封装一个方法，该方法具有四个参数并且不返回值。
    //
    // 参数:
    //   arg1:
    //     此委托封装的方法的第一个参数。
    //
    //   arg2:
    //     此委托封装的方法的第二个参数。
    //
    //   arg3:
    //     此委托封装的方法的第三个参数。
    //
    //   arg4:
    //     此委托封装的方法的第四个参数。
    //
    // 类型参数:
    //   T1:
    //     此委托封装的方法的第一个参数的类型。
    //
    //   T2:
    //     此委托封装的方法的第二个参数的类型。
    //
    //   T3:
    //     此委托封装的方法的第三个参数的类型。
    //
    //   T4:
    //     此委托封装的方法的第四个参数的类型。
    public delegate void EPloyAction<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);


    /// <summary>
    /// 封装一个方法，该方法不具有参数，但却返回 TResult 参数指定的类型的值。
    /// </summary>
    /// <typeparam name="TResult">此委托封装的方法的返回值类型。</typeparam>
    /// <returns>此委托封装的方法的返回值。</returns>
    public delegate TResult EPloyFunc<out TResult>();

    /// <summary>
    /// 封装一个方法，该方法具有一个参数，并返回 TResult 参数所指定的类型的值。
    /// </summary>
    /// <typeparam name="T">此委托封装的方法的参数类型。</typeparam>
    /// <typeparam name="TResult">此委托封装的方法的返回值类型。</typeparam>
    /// <param name="arg">此委托封装的方法的参数。</param>
    /// <returns>此委托封装的方法的返回值。</returns>
    public delegate TResult EPloyFunc<in T, out TResult>(T arg);

    /// <summary>
    /// 封装一个方法，该方法具有两个参数，并返回 TResult 参数所指定的类型的值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="TResult">此委托封装的方法的返回值类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <returns>此委托封装的方法的返回值。</returns>
    public delegate TResult EPloyFunc<in T1, in T2, out TResult>(T1 arg1, T2 arg2);

    /// <summary>
    /// 封装一个方法，该方法具有三个参数，并返回 TResult 参数所指定的类型的值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="TResult">此委托封装的方法的返回值类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <returns>此委托封装的方法的返回值。</returns>
    public delegate TResult EPloyFunc<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// 封装一个方法，该方法具有四个参数，并返回 TResult 参数所指定的类型的值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="TResult">此委托封装的方法的返回值类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <returns>此委托封装的方法的返回值。</returns>
    public delegate TResult EPloyFunc<in T1, in T2, in T3, in T4, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}