﻿using System.Collections.Generic;
using uTinyRipper.YAML;
using uTinyRipper.Converters;
using System;

namespace uTinyRipper.Classes.GUIStyles
{
	public struct GUIStyleState : IAsset
	{
		public GUIStyleState(bool _)
		{
			Background = default;
			TextColor = default;
			ScaledBackgrounds = Array.Empty<PPtr<Texture2D>>();
		}

		public GUIStyleState(GUIStyleState copy)
		{
			Background = copy.Background;
			TextColor = copy.TextColor;
			ScaledBackgrounds = new PPtr<Texture2D>[copy.ScaledBackgrounds.Length];
			for(int i = 0; i < copy.ScaledBackgrounds.Length; i++)
			{
				ScaledBackgrounds[i] = copy.ScaledBackgrounds[i];
			}
		}

		public void Read(AssetReader reader)
		{
			Background.Read(reader);
			ScaledBackgrounds = Array.Empty<PPtr<Texture2D>>();
			//m_scaledBackgrounds = stream.ReadArray<PPtr<Texture2D>>();
			TextColor.Read(reader);
		}

		public void Write(AssetWriter writer)
		{
			Background.Write(writer);
			//writer.WriteAssetArray(m_scaledBackgrounds);
			TextColor.Write(writer);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(BackgroundName, Background.ExportYAML(container));
			node.Add(ScaledBackgroundsName, ScaledBackgrounds.ExportYAML(container));
			node.Add(TextColorName, TextColor.ExportYAML(container));
			return node;
		}

		public IEnumerable<PPtr<Object>> FetchDependencies(DependencyContext context)
		{
			yield break;
		}
		
		public PPtr<Texture2D>[] ScaledBackgrounds { get; set; }

		public const string BackgroundName = "m_Background";
		public const string ScaledBackgroundsName = "m_ScaledBackgrounds";
		public const string TextColorName = "m_TextColor";

		public PPtr<Texture2D> Background;
		public ColorRGBAf TextColor;
	}
}
