namespace EncodingConverter
{
    /// <summary>
    /// 共通のフォーム基底クラス。位置・サイズ保存などの共通処理を提供。
    /// </summary>
    public class FormBase : Form
    {
        /// <summary>
        /// フォームの位置・サイズを保存する（継承先で呼び出し）。
        /// </summary>
        protected virtual void SetFormIcon()
        {
            // フォームのアイコンを設定
            Icon = Properties.Resource.AppIcon;
        }

        ////////////////////////////////
        // 以下、実装例。
        // 継承先で必要に応じてオーバーライドして使用すること。
        //

        /// <summary>
        /// フォームの位置・サイズを保存する（継承先で呼び出し）。
        /// </summary>
        protected virtual void StoreFormState()
        {
            // 例: Properties.Settings.Default.LastLocation = this.Location;
            // 例: Properties.Settings.Default.LastSize = this.Size;
        }

        /// <summary>
        /// フォームの位置・サイズを復元する（継承先で呼び出し）。
        /// </summary>
        protected virtual void RestoreFormState()
        {
            // 例: this.Location = Properties.Settings.Default.LastLocation;
            // 例: this.Size = Properties.Settings.Default.LastSize;
        }

        /// <summary>
        /// 共通の初期化処理（継承先でbase.OnLoad(e)を呼ぶこと）。
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RestoreFormState();
        }

        /// <summary>
        /// 共通の終了処理（継承先でbase.OnFormClosing(e)を呼ぶこと）。
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StoreFormState();
            base.OnFormClosing(e);
        }
    }
}