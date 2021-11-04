﻿using System.Numerics;
using ImGuiNET;

namespace XIVAuras.Config
{
    public class IconStyleConfig : IConfigPage
    {
        public string Name => "Style";

        public void DrawConfig(Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild("##IconStyleConfig", new Vector2(size.X, size.Y), true))
            { 
                ImGui.Text("TODO");

                ImGui.EndChild();
            }
        }
    }
}