﻿namespace WebApp.Models
{
    public class Product
    {
        public int id { get; set; }
        public int code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime sell_date { get; set; }
    }
}