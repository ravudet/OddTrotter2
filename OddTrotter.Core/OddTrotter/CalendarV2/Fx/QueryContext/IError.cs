﻿/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.QueryContext
{
    public interface IError<out TError>
    {
        /// <summary>
        /// placeholder
        /// </summary>
        TError Value { get; }
    }
}
