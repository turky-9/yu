using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows;
using System.Collections.Specialized;

namespace WIN32APIWapper
{
  

    #region enum
    // Indicate whether the method should try to return a name in the pwcsName member of the STATSTG structure
    [Flags]
    public enum STATFLAG
    {
        DEFAULT = 0,
        NONAME = 1,
        NOOPEN = 2
    }

    // Indicate conditions for creating and deleting the object and access modes for the object
    [Flags]
    public enum STGM
    {
        DIRECT = 0x00000000,
        TRANSACTED = 0x00010000,
        SIMPLE = 0x08000000,
        READ = 0x00000000,
        WRITE = 0x00000001,
        READWRITE = 0x00000002,
        SHARE_DENY_NONE = 0x00000040,
        SHARE_DENY_READ = 0x00000030,
        SHARE_DENY_WRITE = 0x00000020,
        SHARE_EXCLUSIVE = 0x00000010,
        PRIORITY = 0x00040000,
        DELETEONRELEASE = 0x04000000,
        NOSCRATCH = 0x00100000,
        CREATE = 0x00001000,
        CONVERT = 0x00020000,
        FAILIFTHERE = 0x00000000,
        NOSNAPSHOT = 0x00200000,
        DIRECT_SWMR = 0x00400000,
    }

    // Indicate whether a storage element is to be moved or copied
    public enum STGMOVE
    {
        MOVE = 0,
        COPY = 1,
        SHALLOWCOPY = 2
    }

    // Specify the conditions for performing the commit operation in the IStorage::Commit and IStream::Commit methods
    [Flags]
    public enum STGC
    {
        DEFAULT = 0,
        OVERWRITE = 1,
        ONLYIFCURRENT = 2,
        DANGEROUSLYCOMMITMERELYTODISKCACHE = 4,
        CONSOLIDATE = 8
    }

    // Specifies the content of the new menu item
    [Flags]
    public enum MIIM : uint
    {
        BITMAP = 0x80,
        CHECKMARKS = 0x08,
        DATA = 0x20,
        FTYPE = 0x100,
        ID = 0x02,
        STATE = 0x01,
        STRING = 0x40,
        SUBMENU = 0x04,
        TYPE = 0x10
    }

    // Specifies the content of the new menu item
    [Flags]
    public enum MFT : uint
    {
        GRAYED = 0x00000003,
        DISABLED = 0x00000003,
        CHECKED = 0x00000008,
        SEPARATOR = 0x00000800,
        RADIOCHECK = 0x00000200,
        BITMAP = 0x00000004,
        OWNERDRAW = 0x00000100,
        MENUBARBREAK = 0x00000020,
        MENUBREAK = 0x00000040,
        RIGHTORDER = 0x00002000,
        BYCOMMAND = 0x00000000,
        BYPOSITION = 0x00000400,
        POPUP = 0x00000010
    }

    // Specifies the state of the new menu item
    [Flags]
    public enum MFS : uint
    {
        GRAYED = 0x00000003,
        DISABLED = 0x00000003,
        CHECKED = 0x00000008,
        HILITE = 0x00000080,
        ENABLED = 0x00000000,
        UNCHECKED = 0x00000000,
        UNHILITE = 0x00000000,
        DEFAULT = 0x00001000
    }

    // Flags used with the CMINVOKECOMMANDINFOEX structure
    [Flags]
    public enum CMIC : uint
    {
        HOTKEY = 0x00000020,
        ICON = 0x00000010,
        FLAG_NO_UI = 0x00000400,
        UNICODE = 0x00004000,
        NO_CONSOLE = 0x00008000,
        ASYNCOK = 0x00100000,
        NOZONECHECKS = 0x00800000,
        SHIFT_DOWN = 0x10000000,
        CONTROL_DOWN = 0x40000000,
        FLAG_LOG_USAGE = 0x04000000,
        PTINVOKE = 0x20000000
    }

    // Specifies how the window is to be shown
    [Flags]
    public enum SW
    {
        HIDE = 0,
        SHOWNORMAL = 1,
        NORMAL = 1,
        SHOWMINIMIZED = 2,
        SHOWMAXIMIZED = 3,
        MAXIMIZE = 3,
        SHOWNOACTIVATE = 4,
        SHOW = 5,
        MINIMIZE = 6,
        SHOWMINNOACTIVE = 7,
        SHOWNA = 8,
        RESTORE = 9,
        SHOWDEFAULT = 10,
    }

    // Window message flags
    [Flags]
    public enum WM : uint
    {
        ACTIVATE = 0x6,
        ACTIVATEAPP = 0x1C,
        AFXFIRST = 0x360,
        AFXLAST = 0x37F,
        APP = 0x8000,
        ASKCBFORMATNAME = 0x30C,
        CANCELJOURNAL = 0x4B,
        CANCELMODE = 0x1F,
        CAPTURECHANGED = 0x215,
        CHANGECBCHAIN = 0x30D,
        CHAR = 0x102,
        CHARTOITEM = 0x2F,
        CHILDACTIVATE = 0x22,
        CLEAR = 0x303,
        CLOSE = 0x10,
        COMMAND = 0x111,
        COMPACTING = 0x41,
        COMPAREITEM = 0x39,
        CONTEXTMENU = 0x7B,
        COPY = 0x301,
        COPYDATA = 0x4A,
        CREATE = 0x1,
        CTLCOLORBTN = 0x135,
        CTLCOLORDLG = 0x136,
        CTLCOLOREDIT = 0x133,
        CTLCOLORLISTBOX = 0x134,
        CTLCOLORMSGBOX = 0x132,
        CTLCOLORSCROLLBAR = 0x137,
        CTLCOLORSTATIC = 0x138,
        CUT = 0x300,
        DEADCHAR = 0x103,
        DELETEITEM = 0x2D,
        DESTROY = 0x2,
        DESTROYCLIPBOARD = 0x307,
        DEVICECHANGE = 0x219,
        DEVMODECHANGE = 0x1B,
        DISPLAYCHANGE = 0x7E,
        DRAWCLIPBOARD = 0x308,
        DRAWITEM = 0x2B,
        DROPFILES = 0x233,
        ENABLE = 0xA,
        ENDSESSION = 0x16,
        ENTERIDLE = 0x121,
        ENTERMENULOOP = 0x211,
        ENTERSIZEMOVE = 0x231,
        ERASEBKGND = 0x14,
        EXITMENULOOP = 0x212,
        EXITSIZEMOVE = 0x232,
        FONTCHANGE = 0x1D,
        GETDLGCODE = 0x87,
        GETFONT = 0x31,
        GETHOTKEY = 0x33,
        GETICON = 0x7F,
        GETMINMAXINFO = 0x24,
        GETOBJECT = 0x3D,
        GETSYSMENU = 0x313,
        GETTEXT = 0xD,
        GETTEXTLENGTH = 0xE,
        HANDHELDFIRST = 0x358,
        HANDHELDLAST = 0x35F,
        HELP = 0x53,
        HOTKEY = 0x312,
        HSCROLL = 0x114,
        HSCROLLCLIPBOARD = 0x30E,
        ICONERASEBKGND = 0x27,
        IME_CHAR = 0x286,
        IME_COMPOSITION = 0x10F,
        IME_COMPOSITIONFULL = 0x284,
        IME_CONTROL = 0x283,
        IME_ENDCOMPOSITION = 0x10E,
        IME_KEYDOWN = 0x290,
        IME_KEYLAST = 0x10F,
        IME_KEYUP = 0x291,
        IME_NOTIFY = 0x282,
        IME_REQUEST = 0x288,
        IME_SELECT = 0x285,
        IME_SETCONTEXT = 0x281,
        IME_STARTCOMPOSITION = 0x10D,
        INITDIALOG = 0x110,
        INITMENU = 0x116,
        INITMENUPOPUP = 0x117,
        INPUTLANGCHANGE = 0x51,
        INPUTLANGCHANGEREQUEST = 0x50,
        KEYDOWN = 0x100,
        KEYFIRST = 0x100,
        KEYLAST = 0x108,
        KEYUP = 0x101,
        KILLFOCUS = 0x8,
        LBUTTONDBLCLK = 0x203,
        LBUTTONDOWN = 0x201,
        LBUTTONUP = 0x202,
        LVM_GETEDITCONTROL = 0x1018,
        LVM_SETIMAGELIST = 0x1003,
        MBUTTONDBLCLK = 0x209,
        MBUTTONDOWN = 0x207,
        MBUTTONUP = 0x208,
        MDIACTIVATE = 0x222,
        MDICASCADE = 0x227,
        MDICREATE = 0x220,
        MDIDESTROY = 0x221,
        MDIGETACTIVE = 0x229,
        MDIICONARRANGE = 0x228,
        MDIMAXIMIZE = 0x225,
        MDINEXT = 0x224,
        MDIREFRESHMENU = 0x234,
        MDIRESTORE = 0x223,
        MDISETMENU = 0x230,
        MDITILE = 0x226,
        MEASUREITEM = 0x2C,
        MENUCHAR = 0x120,
        MENUCOMMAND = 0x126,
        MENUDRAG = 0x123,
        MENUGETOBJECT = 0x124,
        MENURBUTTONUP = 0x122,
        MENUSELECT = 0x11F,
        MOUSEACTIVATE = 0x21,
        MOUSEFIRST = 0x200,
        MOUSEHOVER = 0x2A1,
        MOUSELAST = 0x20A,
        MOUSELEAVE = 0x2A3,
        MOUSEMOVE = 0x200,
        MOUSEWHEEL = 0x20A,
        MOVE = 0x3,
        MOVING = 0x216,
        NCACTIVATE = 0x86,
        NCCALCSIZE = 0x83,
        NCCREATE = 0x81,
        NCDESTROY = 0x82,
        NCHITTEST = 0x84,
        NCLBUTTONDBLCLK = 0xA3,
        NCLBUTTONDOWN = 0xA1,
        NCLBUTTONUP = 0xA2,
        NCMBUTTONDBLCLK = 0xA9,
        NCMBUTTONDOWN = 0xA7,
        NCMBUTTONUP = 0xA8,
        NCMOUSEHOVER = 0x2A0,
        NCMOUSELEAVE = 0x2A2,
        NCMOUSEMOVE = 0xA0,
        NCPAINT = 0x85,
        NCRBUTTONDBLCLK = 0xA6,
        NCRBUTTONDOWN = 0xA4,
        NCRBUTTONUP = 0xA5,
        NEXTDLGCTL = 0x28,
        NEXTMENU = 0x213,
        NOTIFY = 0x4E,
        NOTIFYFORMAT = 0x55,
        NULL = 0x0,
        PAINT = 0xF,
        PAINTCLIPBOARD = 0x309,
        PAINTICON = 0x26,
        PALETTECHANGED = 0x311,
        PALETTEISCHANGING = 0x310,
        PARENTNOTIFY = 0x210,
        PASTE = 0x302,
        PENWINFIRST = 0x380,
        PENWINLAST = 0x38F,
        POWER = 0x48,
        PRINT = 0x317,
        PRINTCLIENT = 0x318,
        QUERYDRAGICON = 0x37,
        QUERYENDSESSION = 0x11,
        QUERYNEWPALETTE = 0x30F,
        QUERYOPEN = 0x13,
        QUEUESYNC = 0x23,
        QUIT = 0x12,
        RBUTTONDBLCLK = 0x206,
        RBUTTONDOWN = 0x204,
        RBUTTONUP = 0x205,
        RENDERALLFORMATS = 0x306,
        RENDERFORMAT = 0x305,
        SETCURSOR = 0x20,
        SETFOCUS = 0x7,
        SETFONT = 0x30,
        SETHOTKEY = 0x32,
        SETICON = 0x80,
        SETMARGINS = 0xD3,
        SETREDRAW = 0xB,
        SETTEXT = 0xC,
        SETTINGCHANGE = 0x1A,
        SHOWWINDOW = 0x18,
        SIZE = 0x5,
        SIZECLIPBOARD = 0x30B,
        SIZING = 0x214,
        SPOOLERSTATUS = 0x2A,
        STYLECHANGED = 0x7D,
        STYLECHANGING = 0x7C,
        SYNCPAINT = 0x88,
        SYSCHAR = 0x106,
        SYSCOLORCHANGE = 0x15,
        SYSCOMMAND = 0x112,
        SYSDEADCHAR = 0x107,
        SYSKEYDOWN = 0x104,
        SYSKEYUP = 0x105,
        TCARD = 0x52,
        TIMECHANGE = 0x1E,
        TIMER = 0x113,
        TVM_GETEDITCONTROL = 0x110F,
        TVM_SETIMAGELIST = 0x1109,
        UNDO = 0x304,
        UNINITMENUPOPUP = 0x125,
        USER = 0x400,
        USERCHANGED = 0x54,
        VKEYTOITEM = 0x2E,
        VSCROLL = 0x115,
        VSCROLLCLIPBOARD = 0x30A,
        WINDOWPOSCHANGED = 0x47,
        WINDOWPOSCHANGING = 0x46,
        WININICHANGE = 0x1A,
        SH_NOTIFY = 0x0401
    }

    /// <summary>
    /// 特殊フォルダのID
    /// </summary>
    public enum SpecialFolderID : uint
    {
        CSIDL_DESKTOP = 0x0000, // デスクトップ
        CSIDL_INTERNET = 0x0001, // インターネット
        CSIDL_PROGRAMS = 0x0002, // プログラム
        CSIDL_CONTROLS = 0x0003, // コントロールパネル
        CSIDL_PRINTERS = 0x0004, // プリンタ
        CSIDL_PERSONAL = 0x0005, // マイ　ドキュメント
        CSIDL_FAVORITES = 0x0006, // お気に入り
        CSIDL_STARTUP = 0x0007, // スタートアップ
        CSIDL_RECENT = 0x0008, // 最近使ったファイル
        CSIDL_SENDTO = 0x0009, // 送る
        CSIDL_BITBUCKET = 0x000a, // ゴミ箱
        CSIDL_STARTMENU = 0x000b, // スタートメニュー
        CSIDL_DESKTOPDIRECTORY = 0x0010, // デスクトップ
        CSIDL_DRIVES = 0x0011, // マイ　コンピュータ
        CSIDL_NETWORK = 0x0012, // マイ　ネットワーク
        CSIDL_NETHOOD = 0x0013, // NetHood
        CSIDL_FONTS = 0x0014, // フォント
        CSIDL_TEMPLATES = 0x0015, // テンプレート
        CSIDL_COMMON_STARTMENU = 0x0016, // 全ユーザーのスタートメニュー
        CSIDL_COMMON_PROGRAMS = 0x0017, // 全ユーザーのプログラム
        CSIDL_COMMON_STARTUP = 0x0018, // 全ユーザーのスタートアップ
        CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019, // 全ユーザーのデスクトップ
        CSIDL_APPDATA = 0x001a, // Application Data
        CSIDL_PRINTHOOD = 0x001b, // PrintHood
        CSIDL_ALTSTARTUP = 0x001d, // ローカライズされないスタートアップ
        CSIDL_COMMON_ALTSTARTUP = 0x001e, // 全ユーザーのローカライズされないスタートアップ
        CSIDL_COMMON_FAVORITES = 0x001f, // 全ユーザーのお気に入り
        CSIDL_INTERNET_CACHE = 0x0020, // IEキャッシュ保存先
        CSIDL_COOKIES = 0x0021, // クッキー
        CSIDL_HISTORY = 0x0022  // 履歴        
    }

    /// <summary>
    /// アイコンのサイズ
    /// </summary>
    public enum EIconSize
    {
        Small,
        Large
    }

    // Specifies how TrackPopupMenuEx positions the shortcut menu horizontally
    [Flags]
    public enum TPM : uint
    {
        LEFTBUTTON = 0x0000,
        RIGHTBUTTON = 0x0002,
        LEFTALIGN = 0x0000,
        CENTERALIGN = 0x0004,
        RIGHTALIGN = 0x0008,
        TOPALIGN = 0x0000,
        VCENTERALIGN = 0x0010,
        BOTTOMALIGN = 0x0020,
        HORIZONTAL = 0x0000,
        VERTICAL = 0x0040,
        NONOTIFY = 0x0080,
        RETURNCMD = 0x0100,
        RECURSE = 0x0001,
        HORPOSANIMATION = 0x0400,
        HORNEGANIMATION = 0x0800,
        VERPOSANIMATION = 0x1000,
        VERNEGANIMATION = 0x2000,
        NOANIMATION = 0x4000,
        LAYOUTRTL = 0x8000
    }

    // Flags indicating which mouse buttons are clicked and which modifier keys are pressed
    [Flags]
    public enum MK
    {
        LBUTTON = 0x0001,
        RBUTTON = 0x0002,
        SHIFT = 0x0004,
        CONTROL = 0x0008,
        MBUTTON = 0x0010,
        ALT = 0x0020
    }

    // Indicates the type of storage medium being used in a data transfer
    [Flags]
    public enum TYMED
    {
        ENHMF = 0x40,
        FILE = 2,
        GDI = 0x10,
        HGLOBAL = 1,
        ISTORAGE = 8,
        ISTREAM = 4,
        MFPICT = 0x20,
        NULL = 0
    }

    // Are used in activation calls to indicate the execution contexts in which an object is to be run
    [Flags]
    public enum CLSCTX : uint
    {
        INPROC_SERVER = 0x1,
        INPROC_HANDLER = 0x2,
        LOCAL_SERVER = 0x4,
        INPROC_SERVER16 = 0x8,
        REMOTE_SERVER = 0x10,
        INPROC_HANDLER16 = 0x20,
        RESERVED1 = 0x40,
        RESERVED2 = 0x80,
        RESERVED3 = 0x100,
        RESERVED4 = 0x200,
        NO_CODE_DOWNLOAD = 0x400,
        RESERVED5 = 0x800,
        NO_CUSTOM_MARSHAL = 0x1000,
        ENABLE_CODE_DOWNLOAD = 0x2000,
        NO_FAILURE_LOG = 0x4000,
        DISABLE_AAA = 0x8000,
        ENABLE_AAA = 0x10000,
        FROM_DEFAULT_CONTEXT = 0x20000,
        INPROC = INPROC_SERVER | INPROC_HANDLER,
        SERVER = INPROC_SERVER | LOCAL_SERVER | REMOTE_SERVER,
        ALL = SERVER | INPROC_HANDLER
    }

    // Directing the handling of the item from which you're retrieving the info tip text
    [Flags]
    public enum QITIPF
    {
        DEFAULT = 0x00000,
        USENAME = 0x00001,
        LINKNOTARGET = 0x00002,
        LINKUSETARGET = 0x00004,
        USESLOWTIP = 0x00008
    }

    // Indicate the type of locking requested for the specified range of bytes
    [Flags]
    public enum LOCKTYPE
    {
        WRITE = 1,
        EXCLUSIVE = 2,
        ONLYONCE = 4
    }


    // The attributes that the caller is requesting, when calling IShellFolder::GetAttributesOf
    [Flags]
    public enum SFGAO : uint
    {
        BROWSABLE = 0x8000000,
        CANCOPY = 1,
        CANDELETE = 0x20,
        CANLINK = 4,
        CANMONIKER = 0x400000,
        CANMOVE = 2,
        CANRENAME = 0x10,
        CAPABILITYMASK = 0x177,
        COMPRESSED = 0x4000000,
        CONTENTSMASK = 0x80000000,
        DISPLAYATTRMASK = 0xfc000,
        DROPTARGET = 0x100,
        ENCRYPTED = 0x2000,
        FILESYSANCESTOR = 0x10000000,
        FILESYSTEM = 0x40000000,
        FOLDER = 0x20000000,
        GHOSTED = 0x8000,
        HASPROPSHEET = 0x40,
        HASSTORAGE = 0x400000,
        HASSUBFOLDER = 0x80000000,
        HIDDEN = 0x80000,
        ISSLOW = 0x4000,
        LINK = 0x10000,
        NEWCONTENT = 0x200000,
        NONENUMERATED = 0x100000,
        READONLY = 0x40000,
        REMOVABLE = 0x2000000,
        SHARE = 0x20000,
        STORAGE = 8,
        STORAGEANCESTOR = 0x800000,
        STORAGECAPMASK = 0x70c50008,
        STREAM = 0x400000,
        VALIDATE = 0x1000000
    }

    // Determines the type of items included in an enumeration. 
    // These values are used with the IShellFolder::EnumObjects method
    [Flags]
    public enum SHCONTF
    {
        FOLDERS = 0x0020,
        NONFOLDERS = 0x0040,
        INCLUDEHIDDEN = 0x0080,
        INIT_ON_FIRST_NEXT = 0x0100,
        NETPRINTERSRCH = 0x0200,
        SHAREABLE = 0x0400,
        STORAGE = 0x0800,
    }


    // Defines the values used with the IShellFolder::GetDisplayNameOf and IShellFolder::SetNameOf 
    // methods to specify the type of file or folder names used by those methods
    [Flags]
    public enum SHGNO
    {
        NORMAL = 0x0000,
        INFOLDER = 0x0001,
        FOREDITING = 0x1000,
        FORADDRESSBAR = 0x4000,
        FORPARSING = 0x8000
    }

    // Flags that specify the file information to retrieve with SHGetFileInfo
    [Flags]
    public enum SHGFI : uint
    {
        ADDOVERLAYS = 0x20,
        ATTR_SPECIFIED = 0x20000,
        ATTRIBUTES = 0x800,
        DISPLAYNAME = 0x200,
        EXETYPE = 0x2000,
        ICON = 0x100,
        ICONLOCATION = 0x1000,
        LARGEICON = 0,
        LINKOVERLAY = 0x8000,
        OPENICON = 2,
        OVERLAYINDEX = 0x40,
        PIDL = 8,
        SELECTED = 0x10000,
        SHELLICONSIZE = 4,
        SMALLICON = 1,
        SYSICONINDEX = 0x4000,
        TYPENAME = 0x400,
        USEFILEATTRIBUTES = 0x10
    }

    // Specifies how the shortcut menu can be changed when calling IContextMenu::QueryContextMenu
    [Flags]
    public enum CMF : uint
    {
        NORMAL = 0x00000000,
        DEFAULTONLY = 0x00000001,
        VERBSONLY = 0x00000002,
        EXPLORE = 0x00000004,
        NOVERBS = 0x00000008,
        CANRENAME = 0x00000010,
        NODEFAULT = 0x00000020,
        INCLUDESTATIC = 0x00000040,
        EXTENDEDVERBS = 0x00000100,
        RESERVED = 0xffff0000
    }

    // Flags specifying the information to return when calling IContextMenu::GetCommandString
    [Flags]
    public enum GCS : uint
    {
        VERBA = 0,
        HELPTEXTA = 1,
        VALIDATEA = 2,
        VERBW = 4,
        HELPTEXTW = 5,
        VALIDATEW = 6
    }

    //SHFileOperation
    public enum FOFunc : uint
    {
        FO_MOVE = 0x0001,
        FO_COPY = 0x0002,
        FO_DELETE = 0x0003,
        FO_RENAME = 0x0004
    }

    //SHFileOperation
    public enum FOFlags : ushort
    {
        FOF_MULTIDESTFILES = 0x0001,
        FOF_CONFIRMMOUSE = 0x0002,
        FOF_SILENT = 0x0004,  // don't create progress/report
        FOF_RENAMEONCOLLISION = 0x0008,
        FOF_NOCONFIRMATION = 0x0010,  // Don't prompt the user.
        FOF_WANTMAPPINGHANDLE = 0x0020,  // Fill in SHFILEOPSTRUCT.hNameMappings
        // Must be freed using SHFreeNameMappings
        FOF_ALLOWUNDO = 0x0040,
        FOF_FILESONLY = 0x0080,  // on *.*, do only files
        FOF_SIMPLEPROGRESS = 0x0100,  // means don't show names of files
        FOF_NOCONFIRMMKDIR = 0x0200,  // don't confirm making any needed dirs
        FOF_NOERRORUI = 0x0400,  // don't put up error UI
        FOF_NOCOPYSECURITYATTRIBS = 0x0800,  // dont copy NT file Security Attributes
        FOF_NORECURSION = 0x1000,  // don't recurse into directories.
        FOF_NO_CONNECTED_ELEMENTS = 0x2000,  // don't operate on connected elements.
        FOF_WANTNUKEWARNING = 0x4000,  // during delete operation, warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
        FOF_NORECURSEREPARSE = 0x8000  // treat reparse points as objects, not containers
    }
    #endregion





    #region struct
    // Represents the number of 100-nanosecond intervals since January 1, 1601
    [StructLayout(LayoutKind.Sequential)]
    public struct FILETIME
    {
        public int dwLowDateTime;
        public int dwHighDateTime;
    }

    // Defines the coordinates of the upper-left and lower-right corners of a rectangle
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct RECT
    {
        public RECT(Rectangle rect)
        {
            left = rect.Left;
            top = rect.Top;
            right = rect.Right;
            bottom = rect.Bottom;
        }

        int left;
        int top;
        int right;
        int bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    // Contains information about a menu item
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct MENUITEMINFO
    {
        public MENUITEMINFO(string text)
        {
            cbSize = Marshal.SizeOf(typeof(MENUITEMINFO));
            dwTypeData = text;
            cch = text.Length;
            fMask = 0;
            fType = 0;
            fState = 0;
            wID = 0;
            hSubMenu = IntPtr.Zero;
            hbmpChecked = IntPtr.Zero;
            hbmpUnchecked = IntPtr.Zero;
            dwItemData = IntPtr.Zero;
            hbmpItem = IntPtr.Zero;
        }

        public int cbSize;
        public MIIM fMask;
        public MFT fType;
        public MFS fState;
        public uint wID;
        public IntPtr hSubMenu;
        public IntPtr hbmpChecked;
        public IntPtr hbmpUnchecked;
        public IntPtr dwItemData;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string dwTypeData;
        public int cch;
        public IntPtr hbmpItem;
    }

    // Contains extended parameters for the TrackPopupMenuEx function
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct TPMPARAMS
    {
        int cbSize;
        RECT rcExclude;
    }

    // Contains extended information about a shortcut menu command
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CMINVOKECOMMANDINFOEX
    {
        public int cbSize;
        public CMIC fMask;
        public IntPtr hwnd;
        public IntPtr lpVerb;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpParameters;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpDirectory;
        public SW nShow;
        public int dwHotKey;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpTitle;
        public IntPtr lpVerbW;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpParametersW;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpDirectoryW;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpTitleW;
        public POINT ptInvoke;
    }

    // Defines the x- and y-coordinates of a point
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct POINT
    {
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x;
        public int y;
    }

    // Contains combo box status information
    [StructLayout(LayoutKind.Sequential)]
    public struct COMBOBOXINFO
    {
        public int cbSize;
        public RECT rcItem;
        public RECT rcButton;
        public IntPtr stateButton;
        public IntPtr hwndCombo;
        public IntPtr hwndEdit;
        public IntPtr hwndList;
    }


    // A generalized global memory handle used for data transfer operations by the 
    // IAdviseSink, IDataObject, and IOleCache interfaces
    [StructLayout(LayoutKind.Sequential)]
    public struct STGMEDIUM
    {
        public TYMED tymed;
        public IntPtr hBitmap;
        public IntPtr hMetaFilePict;
        public IntPtr hEnhMetaFile;
        public IntPtr hGlobal;
        public IntPtr lpszFileName;
        public IntPtr pstm;
        public IntPtr pstg;
        public IntPtr pUnkForRelease;
    }

    //SHFileOperation
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEOPSTRUCT
    {
        public IntPtr hwnd;
        public FOFunc wFunc;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pFrom;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pTo;
        public FOFlags fFlags;
        public bool fAnyOperationsAborted;
        public IntPtr hNameMappings;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszProgressTitle;
    }

    // Contains the information needed to create a drag image
    [StructLayout(LayoutKind.Sequential)]
    public struct SHDRAGIMAGE
    {
        public SIZE sizeDragImage;
        public POINT ptOffset;
        public IntPtr hbmpDragImage;
        public Color crColorKey;
    }

    // The SIZE structure specifies the width and height of a rectangle
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SIZE
    {
        public int cx;
        public int cy;
    }
    #endregion





    #region User32
    /// <summary>
    /// User32ラッパー
    /// </summary>
    public class User32Wrapper
    {
        // DestroyIcon の定義
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        // Sends the specified message to a window or windows
        [DllImport("user32",
            EntryPoint = "SendMessage",
            ExactSpelling = false,
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            WM wMsg,
            int wParam,
            IntPtr lParam);

        // Displays a shortcut menu at the specified location and 
        // tracks the selection of items on the shortcut menu
        [DllImport("user32.dll",
            ExactSpelling = true,
            CharSet = CharSet.Auto)]
        public static extern uint TrackPopupMenuEx(
            IntPtr hmenu,
            TPM flags,
            int x,
            int y,
            IntPtr hwnd,
            IntPtr lptpm);

        // Creates a popup-menu. The menu is initially empty, but it can be filled with 
        // menu items by using the InsertMenuItem, AppendMenu, and InsertMenu functions
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern IntPtr CreatePopupMenu();

        // Destroys the specified menu and frees any memory that the menu occupies
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern bool DestroyMenu(
            IntPtr hMenu);

        // appends a new item to the end of the specified menu bar, drop-down menu, submenu, 
        // or shortcut menu. You can use this function to specify the content, appearance, and 
        // behavior of the menu item
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern bool AppendMenu(
            IntPtr hMenu,
            MFT uFlags,
            uint uIDNewItem,
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpNewItem);

        // Inserts a new menu item into a menu, moving other items down the menu
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern bool InsertMenu(
            IntPtr hmenu,
            uint uPosition,
            MFT uflags,
            uint uIDNewItem,
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpNewItem);

        // Inserts a new menu item at the specified position in a menu
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern bool InsertMenuItem(
            IntPtr hMenu,
            uint uItem,
            bool fByPosition,
            ref MENUITEMINFO lpmii);

        // Deletes a menu item or detaches a submenu from the specified menu
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern bool RemoveMenu(
            IntPtr hMenu,
            uint uPosition,
            MFT uFlags);

        // Retrieves information about a menu item
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern bool GetMenuItemInfo(
            IntPtr hMenu,
            uint uItem,
            bool fByPos,
            ref MENUITEMINFO lpmii);

        // Changes information about a menu item.
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern bool SetMenuItemInfo(
            IntPtr hMenu,
            uint uItem,
            bool fByPos,
            ref MENUITEMINFO lpmii);

        // Determines the default menu item on the specified menu
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern int GetMenuDefaultItem(
            IntPtr hMenu,
            bool fByPos,
            uint gmdiFlags);

        // Sets the default menu item for the specified menu
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern bool SetMenuDefaultItem(
            IntPtr hMenu,
            uint uItem,
            bool fByPos);

        // Retrieves a handle to the drop-down menu or submenu activated by the specified menu item
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern IntPtr GetSubMenu(
            IntPtr hMenu,
            int nPos);

        // Retrieves information about the specified combo box
        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern bool GetComboBoxInfo(
            IntPtr hwndCombo,
            ref COMBOBOXINFO info);
    }
    #endregion


    #region ShlwAPI
    /// <summary>
    /// ShlwAPIラッパー
    /// </summary>
    public class ShlwAPIWrapper
    {
        // Takes a STRRET structure returned by IShellFolder::GetDisplayNameOf,
        // converts it to a string, and places the result in a buffer. 
        [DllImport("shlwapi.dll",
            EntryPoint = "StrRetToBuf",
            ExactSpelling = false,
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern Int32 StrRetToBuf(
            IntPtr pstr,
            IntPtr pidl,
            StringBuilder pszBuf,
            int cchBuf);
    }
    #endregion


    #region Ole32
    public class Ole32Wrapper
    {
        // Registers the specified window as one that can be the target of an OLE drag-and-drop 
        // operation and specifies the IDropTarget instance to use for drop operations
        [DllImport("ole32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern int RegisterDragDrop(
            IntPtr hWnd,
            IDropTarget IdropTgt);

        // Revokes the registration of the specified application window as a potential target for 
        // OLE drag-and-drop operations
        [DllImport("ole32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern int RevokeDragDrop(
            IntPtr hWnd);

        // This function frees the specified storage medium
        [DllImport("ole32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern void ReleaseStgMedium(
            ref STGMEDIUM pmedium);

        // Carries out an OLE drag and drop operation
        [DllImport("ole32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern int DoDragDrop(
            IntPtr pDataObject,
            [MarshalAs(UnmanagedType.Interface)]
            IDropSource pDropSource,
            DragDropEffects dwOKEffect,
            out DragDropEffects pdwEffect);

        // Retrieves a drag/drop helper interface for drawing the drag/drop images
        [DllImport("ole32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern int CoCreateInstance(
            ref Guid rclsid,
            IntPtr pUnkOuter,
            CLSCTX dwClsContext,
            ref Guid riid,
            out IntPtr ppv);

        // Retrieves a data object that you can use to access the contents of the clipboard
        [DllImport("ole32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern int OleGetClipboard(
            out IntPtr ppDataObj);
    }
    #endregion



    #region Shlwapi
    public class ShlwapiWrapper
    {
        // Takes a STRRET structure returned by IShellFolder::GetDisplayNameOf,
        // converts it to a string, and places the result in a buffer. 
        [DllImport("shlwapi.dll",
            EntryPoint = "StrRetToBuf",
            ExactSpelling = false,
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern Int32 StrRetToBuf(
            IntPtr pstr,
            IntPtr pidl,
            StringBuilder pszBuf,
            int cchBuf);
    }
    #endregion


    #region GUIDs
    public class GUIDs
    {
        public static Guid IID_DesktopGUID = new Guid("{00021400-0000-0000-C000-000000000046}");

        public static Guid IID_IShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");
        public static Guid IID_IContextMenu = new Guid("{000214e4-0000-0000-c000-000000000046}");
        public static Guid IID_IContextMenu2 = new Guid("{000214f4-0000-0000-c000-000000000046}");
        public static Guid IID_IContextMenu3 = new Guid("{bcfce0a0-ec17-11d0-8d10-00a0c90f2719}");

        public static Guid IID_IDropTarget = new Guid("{00000122-0000-0000-C000-000000000046}");
        public static Guid IID_IDataObject = new Guid("{0000010e-0000-0000-C000-000000000046}");

        public static Guid IID_IQueryInfo = new Guid("{00021500-0000-0000-C000-000000000046}");
        public static Guid IID_IPersistFile = new Guid("{0000010b-0000-0000-C000-000000000046}");

        public static Guid CLSID_DragDropHelper = new Guid("{4657278A-411B-11d2-839A-00C04FD918D0}");
        public static Guid CLSID_NewMenu = new Guid("{D969A300-E7FF-11d0-A93B-00A0C90F2719}");
        public static Guid IID_IDragSourceHelper = new Guid("{DE5BF786-477A-11d2-839D-00C04FD918D0}");
        public static Guid IID_IDropTargetHelper = new Guid("{4657278B-411B-11d2-839A-00C04FD918D0}");

        public static Guid IID_IShellExtInit = new Guid("{000214e8-0000-0000-c000-000000000046}");
        public static Guid IID_IStream = new Guid("{0000000c-0000-0000-c000-000000000046}");
        public static Guid IID_IStorage = new Guid("{0000000B-0000-0000-C000-000000000046}");
    }
    #endregion

    #region Shell32
    /// <summary>
    /// Shell32ラッパー
    /// </summary>
    public class Shell32Wrapper
    {
       


        public const int MAX_PATH = 260;
        public const uint CMD_FIRST = 1;
        public const uint CMD_LAST = 30000;

        public const int S_OK = 0, S_FALSE = 1;
        public const int DRAGDROP_S_DROP = 0x00040100;
        public const int DRAGDROP_S_CANCEL = 0x00040101;
        public const int DRAGDROP_S_USEDEFAULTCURSORS = 0x00040102;

        public static int cbFileInfo = Marshal.SizeOf(typeof(SHFILEINFO));
        public static int cbMenuItemInfo = Marshal.SizeOf(typeof(MENUITEMINFO));
        public static int cbTpmParams = Marshal.SizeOf(typeof(TPMPARAMS));
        public static int cbInvokeCommand = Marshal.SizeOf(typeof(CMINVOKECOMMANDINFOEX));

        public const uint SHGFI_LARGEICON = 0x00000000;
        public const uint SHGFI_SMALLICON = 0x00000001;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x00000010;
        public const uint SHGFI_ICON = 0x00000100;
        public const uint SHGFI_PIDL = 0x00000008;

        /// <summary>
        /// SHGetMalloc
        /// </summary>
        /// <param name="ppMalloc"></param>
        /// <returns></returns>
        [DllImport("Shell32.DLL")]
        public static extern int SHGetMalloc(out IMalloc ppMalloc);

        /// <summary>
        /// PathからIconを取得する
        /// </summary>
        /// <param name="pszPath"></param>
        /// <param name="dwFileAttributes"></param>
        /// <param name="psfi"></param>
        /// <param name="cbSizeFileInfo"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        /// <summary>
        /// pIDLからIconを取得する
        /// </summary>
        /// <param name="pIDL"></param>
        /// <param name="dwFileAttributes"></param>
        /// <param name="psfi"></param>
        /// <param name="cbSizeFileInfo"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(IntPtr pIDL, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        /// <summary>
        /// 特殊フォルダのpIDLを取得する
        /// </summary>
        /// <param name="hwndOwner"></param>
        /// <param name="nFolder"></param>
        /// <param name="ppidl"></param>
        /// <returns></returns>
        [DllImport("shell32.dll")]
        public static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, int nFolder, out IntPtr ppidl);


        public const int SEE_MASK_INVOKEIDLIST = 0x0000000c;
        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        public static extern int ShellExecuteEx(SHELLEXECUTEINFO shinfo);

        [DllImport("shell32.dll", SetLastError = true)]
        static extern int SHMultiFileProperties(IDataObject pdtobj, int flags);


        // Retrieves the IShellFolder interface for the desktop folder,
        // which is the root of the Shell's namespace. 
        [DllImport("shell32.dll")]
        public static extern Int32 SHGetDesktopFolder(out IntPtr ppshf);


        #region CLRラッパー
        /// <summary>
        /// プロパティ表示の為のCLRラッパー
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="path"></param>
        public static void ShowProperty(IntPtr handle, string path)
        {
            SHELLEXECUTEINFO shinfo = new SHELLEXECUTEINFO();
            shinfo.cbSize = Marshal.SizeOf(typeof(SHELLEXECUTEINFO));
            shinfo.fMask = SEE_MASK_INVOKEIDLIST;
            shinfo.lpVerb = "Properties";
            shinfo.hwnd = (int)handle;
            shinfo.lpParameters = null;
            shinfo.lpDirectory = null;
            shinfo.lpFile = path;

            ShellExecuteEx(shinfo);
        }

        /// <summary>
        /// なんか期待通りの動きをしない、、、
        /// </summary>
        /// <param name="path"></param>
        public static void ShowMultiFileProperties(params string[] path)
        {
            var pdtobj = new DataObject();
            var col = new StringCollection();
            foreach (var s in path)
                col.Add(s);
            pdtobj.SetFileDropList(col);

            SHMultiFileProperties(pdtobj, 0);
        }
        #endregion

        // Unicode を明示的に指定する必要があります。
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation( ref SHFILEOPSTRUCT lpFileOp );
    }
    #endregion


    /// <summary>
    /// IMalloc
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000002-0000-0000-C000-000000000046")]
    public interface IMalloc
    {
        [PreserveSig]
        IntPtr Alloc([In] int cb);
        [PreserveSig]
        IntPtr Realloc([In] IntPtr pv, [In] int cb);
        [PreserveSig]
        void Free([In] IntPtr pv);
        [PreserveSig]
        int GetSize([In] IntPtr pv);
        [PreserveSig]
        int DidAlloc(IntPtr pv);
        [PreserveSig]
        void HeapMinimize();
    }

   

    /// <summary>
    /// Icon取得関数
    /// </summary>
    public class IconFunction
    {
        

        /// <summary>
        /// IMallocをShell32を利用して取得する
        /// </summary>
        private static IMalloc GetSHMalloc()
        {
            IMalloc malloc;
            Shell32Wrapper.SHGetMalloc(out malloc);
            return malloc;
        }

        /// <summary>
        /// 指定されたパスのアイコンを取得する
        /// </summary>
        /// <param name="path">アイコン取得対象のパス</param>
        /// <param name="size">
        /// 取得するアイコンのサイズ
        /// </param>
        /// <param name="isExist">
        /// ファイルが実在する場合だけアイコンを取得する場合 true、実在しなくても拡張子から取得する場合 false
        /// </param>
        /// <returns>取得されたアイコンをIconクラスで返す</returns>
        public static Icon GetFileAssociatedIcon(string path, EIconSize size, bool isExist)
        {
            SHFILEINFO fileInfo = new SHFILEINFO();
            uint flags = Shell32Wrapper.SHGFI_ICON;
            if (size == EIconSize.Small) flags |= Shell32Wrapper.SHGFI_SMALLICON;
            if (!isExist) flags |= Shell32Wrapper.SHGFI_USEFILEATTRIBUTES;
            try
            {
                // アイコンの取得
                Shell32Wrapper.SHGetFileInfo(path, 0, ref fileInfo, (uint)Marshal.SizeOf(fileInfo), flags);
                if (fileInfo.hIcon == IntPtr.Zero)
                    return null;
                else
                {
                    var unmanagedIcon = Icon.FromHandle(fileInfo.hIcon);
                    return new Icon(unmanagedIcon, unmanagedIcon.Size); // unmanagedIconは破棄されるのでManagedで新しく作成する
                }
            }
            finally
            {
                if (fileInfo.hIcon != IntPtr.Zero)
                    User32Wrapper.DestroyIcon(fileInfo.hIcon);
            }
        }

        /// <summary>
        /// 指定した特殊フォルダのアイコンイメージを取得する
        /// </summary>
        /// <param name="targetFolder">アイコンイメージ取得対象の特殊フォルダを指すenum</param>
        /// <param name="size">
        /// 取得するアイコンのサイズ
        /// </param>
        /// <returns>取得されたアイコンをIconクラスで返す</returns>
        public static Icon GetSpecialFolderAssociatedImage(SpecialFolderID targetFolder, EIconSize size)
        {
            uint flags = Shell32Wrapper.SHGFI_ICON | Shell32Wrapper.SHGFI_PIDL;
            if (size == EIconSize.Small) flags |= Shell32Wrapper.SHGFI_SMALLICON;
            SHFILEINFO fileInfo = new SHFILEINFO();
            IntPtr pidlRoot = IntPtr.Zero;
            try
            {
                // 特殊フォルダのpIDLを取得
                Shell32Wrapper.SHGetSpecialFolderLocation(IntPtr.Zero, (int)targetFolder, out pidlRoot);

                // アイコンの取得
                Shell32Wrapper.SHGetFileInfo(pidlRoot, 0, ref fileInfo, (uint)Marshal.SizeOf(fileInfo), flags);
                if (fileInfo.hIcon == IntPtr.Zero)
                    return null;
                else
                {
                    var unmanagedIcon = Icon.FromHandle(fileInfo.hIcon);
                    return new Icon(unmanagedIcon, unmanagedIcon.Size); // unmanagedIconは破棄されるのでManagedで新しく作成する
                }
            }
            finally
            {
                if (fileInfo.hIcon != IntPtr.Zero)
                    User32Wrapper.DestroyIcon(fileInfo.hIcon);

                // pidlを解放する。   
                if (pidlRoot != IntPtr.Zero)
                {
                    IMalloc malloc = GetSHMalloc();
                    malloc.Free(pidlRoot);
                }
            }
        }
    }



    [StructLayout(LayoutKind.Sequential)]
    public class SHELLEXECUTEINFO
    {
        public int cbSize;
        public int fMask;
        public int hwnd;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpVerb;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpFile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpParameters;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpDirectory;
        public int nShow;
        public int hInstApp;
        public int lpIDList;
        public string lpClass;
        public int hkeyClass;
        public int dwHotKey;
        public int hIcon;
        public int hProcess;
    }
}
