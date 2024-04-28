using System;
using System.Text.RegularExpressions;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Visuals;
using TMPro;
using UnityEngine;

namespace Core.Authoring.Behaviours.MapObjects
{
    /// <summary>
    ///   This class defines the behaviour or characters,
    ///   in the sense that it updates its aesthetics and name.
    /// </summary>
    [RequireComponent(typeof(MapObject))]
    public class Character : MonoBehaviour
    {
        private Visual visual;
        
        // Used to match spaces.
        private static Regex rxSpaces = new(@"[\s+]{2,}", RegexOptions.None);
        private const float TextTimeout = 10f;

        // The associated visual's elements.
        private TMP_Text nameObj;
        private TMP_Text textObj;

        // The timeout for the text.
        private float textTimeout = 0f;

        private void Awake()
        {
            visual = GetComponent<MapObject>().MainVisual;
            if (visual)
            {
                nameObj = visual.transform.Find("Name").GetComponent<TMP_Text>();
                textObj = visual.transform.Find("Text").GetComponent<TMP_Text>();
            }
        }

        /// <summary>
        ///   Gets or sets the name of this object.
        /// </summary>
        public string Name
        {
            get => nameObj?.name;
            set
            {
                if (nameObj) nameObj.name = value;
            } 
        }

        /// <summary>
        ///   Sets the text of this object.
        /// </summary>
        /// <param name="text">The text to set</param>
        public void SetText(string text)
        {
            if (!textObj) return;

            text = rxSpaces.Replace(text.Trim(), " ");
            if (text.Length != 0)
            {
                textObj.text = text;
                textTimeout = TextTimeout;
            }
        }

        // Checks for a timeout and clears the message.
        private void Update()
        {
            if (!textObj) return;

            if (textTimeout > 0)
            {
                textTimeout -= Time.deltaTime;
                if (textTimeout <= 0) textObj.text = "";
            }
        }

        private void OnDestroy()
        {
            if (visual) Destroy(visual.gameObject);
        }
    }
}