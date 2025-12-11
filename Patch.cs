using System;
using HarmonyLib;
using it.miketan.PilotSerial.utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace it.miketan.PilotSerial
{
    [HarmonyPatch]
    public class Patch
    {
        // Postfix sul redraw della vista del pilota: aggiunge/riusa una label NGUI con il seriale senza toccare la bio
        [HarmonyPatch(typeof(CIViewBasePilotInfoExtended), "RedrawForPilot")]
        [HarmonyPostfix]
        public static void RedrawForPilot_Postfix(CIViewBasePilotInfoExtended __instance, PersistentEntity pilot)
        {
            InjectForAnchor(__instance?.coreLabelSummary, pilot);
        }

        // Effettua il postfix di InjectForAnchor anche nell'editor (CIViewBaseEditor) in fase di avvio editing
        [HarmonyPatch(typeof(CIViewBaseEditor), "OnEditStart")]
        [HarmonyPostfix]
        public static void Editor_OnEditStart_Postfix(CIViewBaseEditor __instance, PersistentEntity pilot)
        {
            InjectForAnchor(__instance?.inputBio != null ? __instance.inputBio.label : null, pilot);
        }

        // Entry point condiviso: gestisce visibilità/creazione/aggiornamento partendo dall'anchor
        private static void InjectForAnchor(UILabel anchor, PersistentEntity pilot)
        {
            try
            {
                if (pilot == null || !pilot.isPilotTag || pilot.isDestroyed)
                {
                    ToggleSerialLabel(anchor, false);
                    return;
                }

                var sn = PilotSnUtility.GetOrCreate(pilot);
                if (string.IsNullOrEmpty(sn)) return;
                if (anchor == null) return;
                var label = FindOrCreateSerialLabel(anchor, anchor.transform.parent);
                if (label == null) return;

                ApplyStyleAndShow(label, anchor, string.Concat(SerialPrefix, sn));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[PS] Serial label inject failed: {e.Message}");
            }
        }

        private const string SerialNodeName = "PS_Label_Serial";
        private const string SerialPrefix = "P-S/N: ";
        private static readonly Color SerialColor = new Color(0.85f, 0.9f, 1f, 1f);
        private const int SerialVerticalOffset = 18;

        private static UILabel FindOrCreateSerialLabel(UILabel anchor, Transform parent)
        {
            if (anchor == null || parent == null) return null;

            var tf = parent.Find(SerialNodeName);
            UILabel label;
            if (tf == null)
            {
                // Clona l'etichetta esistente per ereditare stile, font, materiali e settaggi NGUI
                var clone = Object.Instantiate(anchor, parent);
                label = clone;
                var go = label.gameObject;
                go.name = SerialNodeName;
                go.layer = 5; // layer UI

                var widget = go.GetComponent<UIWidget>();
                if (widget != null)
                {
                    InitializeAnchors(widget, anchor.transform);
                    // Posiziona sotto l'anchor mantenendo la stessa larghezza
                    widget.bottomAnchor.Set(anchor.transform, 1f, SerialVerticalOffset);
                    widget.topAnchor.Set(anchor.transform, 1f, SerialVerticalOffset + widget.height);
                    widget.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
                    widget.ResetAnchors();
                    widget.UpdateAnchors();
                }

                // Rimuovi eventuali figli temporanei del clone (UILabelSymbols) per farli rigenerare correttamente
                try { NGUITools.DestroyChildren(label.transform); } catch { }
            }
            else
            {
                label = tf.GetComponent<UILabel>();
                if (label == null)
                {
                    Debug.LogWarning("[PS] Found node serial without UILabel: name used from other elements?");
                    return null;
                }
            }

            return label;
        }

        private static void InitializeAnchors(UIWidget widget, Transform t)
        {
            if (widget == null || t == null) return;
            widget.leftAnchor.Set(t, 0f, 0f);
            widget.rightAnchor.Set(t, 1f, 0f);
            widget.topAnchor.Set(t, 1f, 0f);
            widget.bottomAnchor.Set(t, 0f, 0f);
            widget.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
            widget.ResetAnchors();
            widget.UpdateAnchors();
        }

        private static void ApplyStyleAndShow(UILabel label, UILabel anchor, string text)
        {
            label.text = text;
            label.color = SerialColor;
            label.fontSize = anchor.fontSize; // mantiene lo stile del font usato nel gioco.
            label.depth = anchor.depth + 1;
            label.gameObject.SetActive(true);
        }

        private static void ToggleSerialLabel(UILabel anchor, bool visible)
        {
            if (anchor == null) return;
            var parent = anchor.transform?.parent;
            if (parent == null) return;
            var tf = parent.Find(SerialNodeName);
            if (tf != null)
            {
                tf.gameObject.SetActive(visible);
            }
        }
        // Fine helpers
    }
}