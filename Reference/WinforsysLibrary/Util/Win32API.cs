using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

namespace Winforsys.Util
{
    /// <summary>
    /// Win32 API를 사용하기 위한 Import 객체
    /// </summary>
    public class Win32API
    {
        /// <summary>
        /// 프로그램 중복 생성을 방지 하기 위한 함수
        /// </summary>
        /// <param name="lpEventAttributes"></param>
        /// <param name="bManualReset"></param>
        /// <param name="bInitialState"></param>
        /// <param name="lpName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        /// <summary>
        /// 프로그램 중복 실행 시 발생하는 메세지를 구분하기 위한 함수
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetLastError();

        /// <summary>
        /// 프로그램 중복 실행 Error
        /// </summary>
        public const int ERROR_ALREADY_EXISTS = 183;       
    }
}
