using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System.Windows;

namespace YuFiler
{
    public class KeyBind
    {
        public event EventHandler<KeyBindInternalEventArgs> InternalKeyBindExecute;

        protected Dictionary<Tuple<Key, ModifierKeys>, KeyBindItem> Dic = new Dictionary<Tuple<Key, ModifierKeys>, KeyBindItem>();

        public KeyBind()
        {
        }

        public void AddItem(KeyBindItem item)
        {
            Dic.Add(new Tuple<Key,ModifierKeys>(item.PushDownKey,item.Modifiers), item);
        }

        public bool Execute(Key key, params FileSystemInfo[] items)
        {
            Tuple<Key, ModifierKeys> tuplekey = new Tuple<Key, ModifierKeys>(key, Keyboard.Modifiers);
            if (this.Dic.ContainsKey(tuplekey))
            {
                KeyBindItem command = this.Dic[tuplekey];
                if (command.Modifiers == Keyboard.Modifiers)
                {
                    if (command.ExecKbn == EKeyBindExecKbn.External)
                    {
                        //外部コマンド実行
                        try
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < items.Length; i++)
                            {
                                if (i != 0)
                                    sb.Append(" ");
                                sb.Append(items[i].FullName);
                            }
                            ProcessStartInfo psinfo = new ProcessStartInfo();
                            psinfo.FileName = command.Command;
                            psinfo.CreateNoWindow = false;
                            psinfo.UseShellExecute = false;
                            psinfo.Arguments = sb.ToString();
                            Process proc = Process.Start(psinfo);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        if (this.InternalKeyBindExecute != null)
                            this.InternalKeyBindExecute(this, new KeyBindInternalEventArgs(command.InternalKbn));
                    }
                }
            }
            return false;
        }
    }

    public class KeyBindItem
    {
        public Key PushDownKey { get; set; }
        public ModifierKeys Modifiers { get; set; }
        public EKeyBindExecKbn ExecKbn { get; set; }
        public string Command { get; set; }
        public EKeyBindInternalKbn InternalKbn { get; set; }
    }

    public enum EKeyBindExecKbn
    {
        Internal = 0,
        External
    }

    public enum EKeyBindInternalKbn
    {
        ReDisp = 0
    }

    public class KeyBindInternalEventArgs : EventArgs
    {
        public EKeyBindInternalKbn Kbn { get; private set; }

        public KeyBindInternalEventArgs(EKeyBindInternalKbn kbn)
        {
            this.Kbn = kbn;
        }
    }
}
