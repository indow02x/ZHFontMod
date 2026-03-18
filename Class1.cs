using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.Fonts;
using System.Reflection; // 引入反射功能，用來尋找 DLL 自己的位置

namespace ZHFontMod
{
    [HarmonyPatch(typeof(FontManager), "GetFontForLanguage")]
    public class GlobalFontOverridePatch
    {
        // 宣告一個靜態變數把字體存起來，節省效能
        static FontFile myCustomFont = null;

        static bool Prefix(ref Font __result, string language, FontType type)
        {
            // 如果遊戲索取的是簡體中文 (zhs)
            if (language == "zhs") 
            {
                // 如果字體還沒被載入過，就進行載入
                if (myCustomFont == null)
                {
                    myCustomFont = new FontFile();
                    
                    // 取得當前這支 Mod DLL 檔案的完整路徑
                    string dllPath = Assembly.GetExecutingAssembly().Location;
                    
                    // 擷取出這個 DLL 所在的資料夾路徑
                    string modDirectory = Path.GetDirectoryName(dllPath);
                    
                    // 把資料夾路徑跟字體檔名接在一起 (自動尋找同目錄下的 zh_font.ttf)
                    string fontPath = Path.Combine(modDirectory, "zh_font.ttf");
                    
                    // 讀取字體
                    Error err = myCustomFont.LoadDynamicFont(fontPath);
                    
                    if (err != Error.Ok)
                    {
                        GD.PrintErr($"[FontMod] 讀取字體失敗！請確認 {fontPath} 是否存在。錯誤碼: {err}");
                    }
                    else
                    {
                        GD.Print($"[FontMod] 成功從 Mod 資料夾讀取字體！路徑: {fontPath}");
                    }
                }
                
                // 把記憶體裡的字體物件回傳給遊戲
                __result = myCustomFont;
                
                // 回傳 false 阻斷原廠邏輯
                return false; 
            }
            
            // 如果遊戲要的是英文或其他語言就放行
            return true;
        }
    }
}