﻿using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using Newtonsoft.Json;
using XIVAuras.Config;
using XIVAuras.Helpers;

namespace XIVAuras.Auras
{
    public abstract class AuraListItem : IConfigurable
    {
        [JsonIgnore] public readonly string ID;
        [JsonIgnore] protected bool LastFrameWasPreview = false;
        [JsonIgnore] protected bool LastFrameWasDragging = false;
        [JsonIgnore] public bool Preview = false;
        [JsonIgnore] public bool Hovered = false;
        [JsonIgnore] public bool Dragging = false;
        [JsonIgnore] public bool SetPosition = false;
        [JsonIgnore] protected DataSource? StartData = null;
        [JsonIgnore] protected DateTime? StartTime = null;
        [JsonIgnore] protected DataSource? OldStartData = null;
        [JsonIgnore] protected DateTime? OldStartTime = null;

        public string Name { get; set; }

        public AuraListItem(string name)
        {
            this.Name = name;
            this.ID = $"XIVAuras_{GetType().Name}_{Guid.NewGuid()}";
        }

        public abstract AuraType Type { get; }

        public abstract void Draw(Vector2 pos, Vector2? parentSize = null);

        public abstract IEnumerable<IConfigPage> GetConfigPages();

        public override string? ToString() => $"{this.Type} [{this.Name}]";

        public virtual void StopPreview()
        {
            this.Preview = false;
        }

        protected DataSource UpdatePreviewData(DataSource data)
        {
            if (this.StartTime.HasValue && this.StartData is not null)
            {
                float secondSinceStart = (float)(DateTime.UtcNow - this.StartTime.Value).TotalSeconds;
                float resetValue = Math.Min(this.StartData.Value, this.StartData.Value);
                float newValue = resetValue - secondSinceStart;

                if (newValue < 0)
                {
                    this.StartTime = DateTime.UtcNow;
                    newValue = resetValue;
                }

                return new DataSource()
                {
                    Value = newValue,
                    Stacks = data.Stacks
                };
            }

            return data;
        }

        // Dont ask
        protected void UpdateDragData(Vector2 pos, Vector2 size)
        {
            this.Hovered = ImGui.IsMouseHoveringRect(pos, pos + size);
            this.Dragging = this.LastFrameWasDragging && ImGui.IsMouseDown(ImGuiMouseButton.Left);
            this.SetPosition = (this.Preview && !this.LastFrameWasPreview || !this.Hovered) && !this.Dragging;
            this.LastFrameWasDragging = this.Hovered || this.Dragging;
        }

        protected void UpdateStartData(DataSource data, TriggerType type)
        {
            if (this.LastFrameWasPreview && !this.Preview)
            {
                this.StartData = this.OldStartData;
                this.StartTime = this.OldStartTime;
            }
            
            if (!this.LastFrameWasPreview && this.Preview)
            {
                this.OldStartData = this.StartData;
                this.OldStartTime = this.StartTime;
                this.StartData = null;
                this.StartTime = null;
            }

            if (this.StartData is not null &&
                data.Value > this.StartData.Value)
            {
                this.StartData = data;
                this.StartTime = DateTime.UtcNow;
            }

            if (this.StartData is null || !this.StartTime.HasValue)
            {
                this.StartData = data;
                this.StartTime = DateTime.UtcNow;
            }
        }
    }

    public enum AuraType
    {
        Group,
        Icon,
        Bar,
        Label
    }
}