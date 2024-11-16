using UnityEngine;

[CreateAssetMenu(fileName = "skinManager", menuName = "Skin Manager")]
public class SkinManager : ScriptableObject
{
    [SerializeField] public Skin[] skins;
    private const string Prefix = "Skin_";
    private const string SelectedSkin = "SelectedSkin";

    //Select bought skin
    public void SelectSkin(int skinIndex)
    {
        PlayerPrefs.SetInt(SelectedSkin, skinIndex);
        ShopController.instance.UpdatePlayerSprite();
        ShopController.instance.UpdateShopText();
    }

    //Gets The skin you select
    public Skin GetSelectedSkin()
    {
        var skinIndex = PlayerPrefs.GetInt(SelectedSkin, 0);
        if(skinIndex >= 0 && skinIndex < skins.Length)
        {
            return skins[skinIndex];
        }

        return null;
    }

    //Unlocks skin
    public void Unlock(int skinIndex)
    {
        PlayerPrefs.SetInt(Prefix + skinIndex, 1);
        ShopController.instance.UpdatePlayerSprite();
        ShopController.instance.UpdateShopText();
        SelectSkin(skinIndex);
    }

    //Locks Skin (only used in extramenu)
    public void Lock(int skinIndex) => PlayerPrefs.SetInt(Prefix + skinIndex, 0);

    //Checks if skin is locked/unlocked
    public bool IsUnlocked(int skinIndex) => PlayerPrefs.GetInt(Prefix + skinIndex, 0) == 1;
}
