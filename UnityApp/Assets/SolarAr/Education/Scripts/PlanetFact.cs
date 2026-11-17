using System;
using UnityEngine;

namespace SolarAR.Education
{
    [Serializable]
    public sealed class PlanetFact
    {
        [SerializeField] private string _id;
        [SerializeField] private string _title;
        [SerializeField, TextArea(2, 5)] private string _description;
        [SerializeField] private PlanetFactCategory _category;
        [SerializeField, Min(1)] private int _order = 1;
        [SerializeField] private Sprite _visual;

        public string Id => _id;
        public string Title => _title;
        public string Description => _description;
        public PlanetFactCategory Category => _category;
        public int Order => _order;
        public Sprite Visual => _visual;
        public string CategoryLabel => PlanetFactCategoryLabels.GetLabel(_category);

        public PlanetFact(
            string id,
            string title,
            string description,
            PlanetFactCategory category,
            int order,
            Sprite visual = null)
        {
            _id = id;
            _title = title;
            _description = description;
            _category = category;
            _order = order;
            _visual = visual;
        }
    }
}
