using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.TemplateGen
{
    public static class GenHelper
    {

        /// <summary>
        /// Captures local variables for closure function.
        /// More info: https://unicorn-dev.medium.com/how-to-capture-a-variable-in-c-and-not-to-shoot-yourself-in-the-foot-d169aa161aa6
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="locals"></param>
        /// <returns></returns>
        static public Func<TResult> WrapFunc<TParam, TResult>(Func<TParam, TResult> func, TParam locals)
        {
            return () => func(locals);
        }
    }
}
