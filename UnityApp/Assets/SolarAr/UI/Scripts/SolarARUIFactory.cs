using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolarAR.UI
{
    internal static class SolarARColors
    {
        public static readonly Color Night = Hex("0A1433");
        public static readonly Color Ink = Hex("19365B");
        public static readonly Color Blue1 = Hex("387ED8");
        public static readonly Color Blue4 = Hex("294671");
        public static readonly Color OnDark = Color.white;
        public static readonly Color Panel = Hex("F7FAFF");
        public static readonly Color Primary = Hex("FFDB69");
        public static readonly Color Cyan = Hex("69D9EF");
        public static readonly Color Secondary = Hex("97E4F4");
        public static readonly Color Lavender = Hex("D4D1F3");
        public static readonly Color FactSun = Hex("FFF4CD");
        public static readonly Color Success = Hex("B6E9C9");

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString($"#{value}", out Color color);
            return color;
        }
    }

    internal sealed class SolarARUIFactory
    {
        private readonly TMP_FontAsset _titleFont;
        private readonly TMP_FontAsset _bodyFont;

        public SolarARUIFactory(TMP_FontAsset titleFont, TMP_FontAsset bodyFont)
        {
            _titleFont = titleFont;
            _bodyFont = bodyFont;
        }

        public RectTransform Rect(string name, Transform parent)
        {
            GameObject instance = new(name, typeof(RectTransform));
            RectTransform rect = instance.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        public RectTransform Panel(
            string name,
            Transform parent,
            Color color,
            float radius = 0f)
        {
            GameObject instance = new(name, typeof(RectTransform), typeof(CanvasRenderer));
            RectTransform rect = instance.GetComponent<RectTransform>();
            rect.SetParent(parent, false);

            if (radius > 0f)
            {
                RoundedRectangleGraphic graphic = instance.AddComponent<RoundedRectangleGraphic>();
                graphic.color = color;
                graphic.Radius = radius;
            }
            else
            {
                Image image = instance.AddComponent<Image>();
                image.color = color;
            }

            return rect;
        }

        public RectTransform Circle(string name, Transform parent, Color color, float diameter)
        {
            RectTransform rect = Panel(name, parent, color, diameter * 0.5f);
            rect.sizeDelta = Vector2.one * diameter;
            return rect;
        }

        public TextMeshProUGUI Text(
            string name,
            Transform parent,
            string value,
            float fontSize,
            Color color,
            TextAlignmentOptions alignment = TextAlignmentOptions.Center,
            bool title = false,
            FontStyles style = FontStyles.Normal)
        {
            GameObject instance = new(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI));
            RectTransform rect = instance.GetComponent<RectTransform>();
            rect.SetParent(parent, false);

            TextMeshProUGUI text = instance.GetComponent<TextMeshProUGUI>();
            text.text = value;
            text.font = title ? _titleFont : _bodyFont;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.fontStyle = style;
            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Truncate;
            text.raycastTarget = false;
            return text;
        }

        public Button Button(
            string name,
            Transform parent,
            string label,
            Color background,
            Color foreground,
            UnityAction action,
            float radius = 24f,
            float fontSize = 20f,
            bool title = false)
        {
            RectTransform rect = Panel(name, parent, background, radius);
            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = rect.GetComponent<Graphic>();
            button.transition = Selectable.Transition.ColorTint;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.96f, 0.96f, 0.96f, 1f);
            colors.pressedColor = new Color(0.84f, 0.84f, 0.84f, 1f);
            colors.disabledColor = new Color(1f, 1f, 1f, 0.45f);
            button.colors = colors;
            if (action != null)
            {
                button.onClick.AddListener(action);
            }

            TextMeshProUGUI text = Text(
                "Label",
                rect,
                label,
                fontSize,
                foreground,
                TextAlignmentOptions.Center,
                title,
                FontStyles.SemiBold);
            Stretch(text.rectTransform, 12f, 6f, 12f, 6f);
            return button;
        }

        public Button IconButton(
            string name,
            Transform parent,
            string symbol,
            UnityAction action,
            Color background,
            Color foreground,
            float diameter = 48f)
        {
            Button button = Button(
                name,
                parent,
                symbol,
                background,
                foreground,
                action,
                diameter * 0.5f,
                25f,
                true);
            button.GetComponent<RectTransform>().sizeDelta = Vector2.one * diameter;
            return button;
        }

        public Image Line(string name, Transform parent, Color color)
        {
            GameObject instance = new(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            instance.transform.SetParent(parent, false);
            Image image = instance.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        public static void Stretch(
            RectTransform rect,
            float left = 0f,
            float bottom = 0f,
            float right = 0f,
            float top = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }

        public static void Place(
            RectTransform rect,
            float x,
            float y,
            float width,
            float height,
            Vector2 anchor,
            Vector2 pivot)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        public static void Top(
            RectTransform rect,
            float top,
            float height,
            float left = 0f,
            float right = 0f)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2((left - right) * 0.5f, -top);
            rect.sizeDelta = new Vector2(-(left + right), height);
        }

        public static void Bottom(
            RectTransform rect,
            float bottom,
            float height,
            float left = 0f,
            float right = 0f)
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2((left - right) * 0.5f, bottom);
            rect.sizeDelta = new Vector2(-(left + right), height);
        }
    }
}
