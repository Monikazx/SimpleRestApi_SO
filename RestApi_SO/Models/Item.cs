﻿namespace RestApiSO.Models
{
    public class Item
    {
        public LinkedList<Tag> items { get; set; }
        public bool has_more { get; set; }
        public int quota_max { get; set; }
        public int quota_remaining { get; set; }
    }
}
