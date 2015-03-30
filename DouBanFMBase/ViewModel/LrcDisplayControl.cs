using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DouBanFMBase.ViewModel
{
    public class LrcDisplayControl:Control
    {
        private ScrollViewer RootScrollViewer;
        private Grid RootGrid;
        private TextBlock TopTextBlock;
        private TextBlock MidTextBlock;
        private TextBlock BottomRowText;
        public int CurFramentIndex { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RootScrollViewer = GetTemplateChild("RootScrollViewer") as ScrollViewer;
            RootGrid = GetTemplateChild("RootGrid") as Grid; 
            TopTextBlock = GetTemplateChild("TopRowText") as TextBlock; 
            MidTextBlock = GetTemplateChild("MidRowText") as TextBlock;
            BottomRowText = GetTemplateChild("BottomRowText") as TextBlock;
        }

        #region properties
        public static Lyric Lrc { get; set; }

        public string LrcText
        {
            get
            { 
                return (string)GetValue(LrcTextProperty);
            }
            set
            { 
                SetValue(LrcTextProperty,value);
            }
        }
        public static readonly DependencyProperty LrcTextProperty = DependencyProperty.Register("LrcText",typeof(string),typeof(LrcDisplayControl),new PropertyMetadata( new PropertyChangedCallback(OnLrcTextPropertyChanged)));

        private static void OnLrcTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LrcDisplayControl lrcControl = d as LrcDisplayControl;
            if (lrcControl != null)
            {
                lrcControl.UpdateLrcText();
            }
        }
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(LrcDisplayControl), new PropertyMetadata(new PropertyChangedCallback(OnProgressPropertyChanged)));
        private static void OnProgressPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            LrcDisplayControl lrcControl = sender as LrcDisplayControl;
            if (lrcControl != null)
            {
                //进度变化了，调用控件相应方法，通知其更新
                lrcControl.OnProgressChanged();
            }
        }
        /// <summary> /// 
        /// 歌词高亮Brush /// 
        /// </summary>
        public Brush EmphasisBrush
        {
            get { return (Brush)GetValue(EmphasisBrushProperty); }
            set { SetValue(EmphasisBrushProperty, value); }
        }
        public static readonly DependencyProperty EmphasisBrushProperty = DependencyProperty.Register("EmphasisBrush", typeof(Brush), typeof(LrcDisplayControl), null);
        /// <summary> 
        /// 歌词行高度 ///
        /// </summary> 
        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }
        public static readonly DependencyProperty LineHeightProperty = DependencyProperty.Register("LineHeight", typeof(double), typeof(LrcDisplayControl), null);

        #endregion

        #region control helper Method
        private void UpdateLrcText()
        {
            if (LrcText == null)
            {
                //歌词为空 清空相关数据
                Lrc = null;
                if (TopTextBlock != null)
                {
                    TopTextBlock.Text = MidTextBlock.Text = BottomRowText.Text = null;
                }
                return;
            }

            //歌词变化 重新解析歌词
            Lrc = new Lyric(LrcText);
            if (TopTextBlock != null)
            {
                TopTextBlock.Text = MidTextBlock.Text = null;
                if (Lrc.Fragments != null)
                {
                    BottomRowText.Text = ComposeLrcFraments(0);
                }
                else
                {
                    BottomRowText.Text = null;
                }
            }
        }
       
               //播放进度变化时调用，调整控件?
        private void OnProgressChanged() 
        {
            if (Lrc == null || Lrc.Fragments.Count == 0) 
            { return; } 
            int curFragIndex; //查找当前播放进度下应显示的歌词片段index,如果和当前显示的index不同，则更新
            if (Lrc.GetFragmentIndex(Progress, out curFragIndex) && curFragIndex != CurFramentIndex)
            {
                TopTextBlock.Text = ComposeLrcFraments(0, curFragIndex); 
                MidTextBlock.Text = Lrc.Fragments[curFragIndex].LrcText;
                BottomRowText.Text = ComposeLrcFraments(curFragIndex + 1); 
                CurFramentIndex = curFragIndex; 
                UpdateVerticalScroll(); 
            }
        }

        private void UpdateVerticalScroll()
        {
            double verticalScrollOffset = TopTextBlock.ActualHeight - RootScrollViewer.ActualHeight / 2
                                + MidTextBlock.ActualHeight / 2;
            RootScrollViewer.ScrollToVerticalOffset(verticalScrollOffset);
        }
        private string ComposeLrcFraments(int p, int index = 0)
        {
            string framents = null;
            if (index == 0)
            {
                index = Lrc.Fragments.Count;
            }
            for (int i = p; i < index; i++)
            {
                framents += Lrc.Fragments[i].LrcText;
            }
            return framents;
        }
        #endregion
    }
}
