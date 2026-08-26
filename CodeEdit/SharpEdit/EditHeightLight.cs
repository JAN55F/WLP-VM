using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastColoredTextBoxNS;
using System.Text.RegularExpressions;
using System.Drawing;

namespace SharpEdit
{
    class EditHeightLight
    {
        //styles
        public static TextStyle KeyWordsStyle = new TextStyle(Brushes.RoyalBlue, null, FontStyle.Regular);//关键字
        public static TextStyle ClassNameStyle = new TextStyle(Brushes.Orange, null, FontStyle.Bold);//类名,结构体名称
        public static TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        public static TextStyle NumberStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        public static TextStyle NoteStyle = new TextStyle(Brushes.DarkGreen, null, FontStyle.Italic);//注释
        public static TextStyle StringStyle = new TextStyle(Brushes.Lime, null, FontStyle.Italic);//字符串引号
        public static TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        public static MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

      public static void CSharpSyntaxHighlight(FastColoredTextBox fctb, TextChangedEventArgs e)
      {
          fctb.LeftBracket = '(';
          fctb.RightBracket = ')';
          fctb.LeftBracket2 = '\x0';
          fctb.RightBracket2 = '\x0';
          //clear style of changed range
          e.ChangedRange.ClearStyle(KeyWordsStyle, ClassNameStyle, GrayStyle, NumberStyle, NoteStyle, StringStyle);

          //string highlighting
          e.ChangedRange.SetStyle(StringStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
          //comment highlighting
          e.ChangedRange.SetStyle(NoteStyle, @"//.*$", RegexOptions.Multiline);
          e.ChangedRange.SetStyle(NoteStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
          e.ChangedRange.SetStyle(NoteStyle, @"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);
          //number highlighting
          e.ChangedRange.SetStyle(NumberStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
          //attribute highlighting
          e.ChangedRange.SetStyle(GrayStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
          //class name highlighting
          e.ChangedRange.SetStyle(ClassNameStyle, @"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b");
          //keyword highlighting
          e.ChangedRange.SetStyle(KeyWordsStyle, @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\b|#region\b|#endregion\b");

          //clear folding markers
          e.ChangedRange.ClearFoldingMarkers();
          //set folding markers
          e.ChangedRange.SetFoldingMarkers("{", "}");//allow to collapse brackets block
          e.ChangedRange.SetFoldingMarkers(@"#region\b", @"#endregion\b");//allow to collapse #region blocks
          e.ChangedRange.SetFoldingMarkers(@"/\*", @"\*/");//allow to collapse comment block
      }
    }

}
