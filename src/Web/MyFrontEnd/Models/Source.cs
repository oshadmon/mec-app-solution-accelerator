﻿using System;
namespace MyFrontEnd.Models
{
	public class Source
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public double Lat{ get; set; }
		public double Long { get; set; }

		public Source(string id, string name, string type, double latitude, double longitude)
		{
			Id = id;
			Name = name;
			Type = type;
			Lat = latitude;
			Long = longitude;
		}
	}
}

