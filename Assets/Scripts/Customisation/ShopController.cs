using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController instance;
    private void Awake() => instance = this;

    [SerializeField] private SkinManager skinManager;
    [SerializeField] private Renderer playerSprite, playerSpriteShop;
    [SerializeField] private TMP_Text skinNameText, skinDescriptionText, acornAmountText;
    [SerializeField] private Button BuyButton;

    private int _currentSkin;

    private void Start()
    {
        _currentSkin = PlayerPrefs.GetInt("SelectedSkin");
        UpdatePlayerSprite();
        UpdateShopText();
    }

    public void UpdatePlayerSprite()
    {
        var playerSpriteSkin = skinManager.GetSelectedSkin();
        var playerSpriteShopSkin = skinManager.skins[_currentSkin];
        
        playerSprite.materials = playerSpriteSkin.materials;
        playerSpriteShop.materials = playerSpriteShopSkin.materials;
    }

    public void UpdateShopText()
    {
        if (BuyButton == null) return;
        
        if (skinManager.IsUnlocked(_currentSkin) && _currentSkin != PlayerPrefs.GetInt("SelectedSkin"))
        {
            BuyButton.GetComponentInChildren<TMP_Text>().text = "Select";
        }
        else if (_currentSkin == PlayerPrefs.GetInt("SelectedSkin"))
        {
            BuyButton.GetComponentInChildren<TMP_Text>().text = "Selected";
        }
        else
        {
            BuyButton.GetComponentInChildren<TMP_Text>().text = $"Buy for {skinManager.skins[_currentSkin].cost}";
        }
        
        skinNameText.text = skinManager.skins[_currentSkin].name;
        skinDescriptionText.text = skinManager.skins[_currentSkin].description;
    }

    private void CheckFreeSkin(int skinID)
    {
        if (skinManager.IsUnlocked(skinID) || skinManager.skins[skinID].cost != 0) return;
        skinManager.Unlock(skinID);
    }

    public void SelectNextSkin()
    {
        if (_currentSkin + 1 == skinManager.skins.Length) return;
        _currentSkin++;
        UpdatePlayerSprite();
        UpdateShopText();
        CheckFreeSkin(_currentSkin);
    }

    public void SelectPreviousSkin()
    {
        if (_currentSkin - 1 < 0) return;
        _currentSkin--;
        UpdatePlayerSprite();
        UpdateShopText();
        CheckFreeSkin(_currentSkin);
    }

    public void OnBuyButtonPress()
    {
        if (skinManager.IsUnlocked(_currentSkin)) // if cost is 0, unlock it when rendering as per default
        {
            skinManager.SelectSkin(_currentSkin);
        }
        else
        {
            var coins = PlayerPrefs.GetInt("coins", 0);

            if (coins >= skinManager.skins[_currentSkin].cost && !skinManager.IsUnlocked(_currentSkin))
            {
                PlayerPrefs.SetInt("coins", coins - skinManager.skins[_currentSkin].cost);
                skinManager.Unlock(_currentSkin);
                skinManager.SelectSkin(_currentSkin);
                BlinkBuyButtonText(Color.green);
                acornAmountText.text = PlayerPrefs.GetInt("coins", 0).ToString();
            }
            else
            {
                BlinkBuyButtonText(Color.red);
            }
        }

        UpdatePlayerSprite();
        UpdateShopText();
    }

    private void BlinkBuyButtonText(Color col)
    {
        UpdatePlayerSprite();
        UpdateShopText();
        BuyButton.GetComponentInChildren<TMP_Text>().color = col;
        DG.Tweening.DOTweenModuleUI.DOColor(BuyButton.GetComponentInChildren<TMP_Text>(), Color.white, 0.6f);
    }
}