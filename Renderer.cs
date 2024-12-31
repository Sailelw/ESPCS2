using ClickableTransparentOverlay;
using ImGuiNET;
using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ESPCS2
{
    public class Renderer : Overlay
    {

        //radom stuff
        #region

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int key);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowname);

        [DllImport("user32.dll")]
        static extern short SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        enum WDA
        {
            WDA_NONE = 0x00000000,
            WDA_MONITOR = 0x00000001,
            WDA_EXCLUDEFROMCAPTURE = 0x00000011
        }

        #endregion

        // hide region
        #region
        public static void Hide()
        {
            string overlay = "Overlay";
            IntPtr OverlayHwnd = FindWindow(null, overlay);
            if (isBypassStream)
            {
                SetWindowDisplayAffinity(OverlayHwnd, (uint)WDA.WDA_EXCLUDEFROMCAPTURE);
            }
            else
            {
                SetWindowDisplayAffinity(OverlayHwnd, (uint)WDA.WDA_NONE);
            }
        }
        #endregion

        // Vector stuff
        #region
        public Vector2 screenSize = new Vector2(2560, 1440);
        public Vector4 circleColor = new Vector4(1, 1, 1, 0.5f);
        public Vector2 sd3 = new Vector2(500, 400);
        public Vector4 enemyColor = new Vector4(1, 0, 0, 1); //default red
        public Vector4 teamColor = new Vector4(0, 1, 0, 1); //default green
        public Vector4 nameColor = new Vector4(1, 1, 1, 1); //white
        public Vector4 boneColor = new Vector4(1, 1, 1, 1);
        public Vector4 teamColorBone = new Vector4(1,1,1,1);
        public Vector4 white = new Vector4(1, 1, 1, 1);
        public Vector4 green = new Vector4(0, 1 , 0, 1);
        public Vector4 yellow = new Vector4(1, 1, 0, 1);
        public Vector4 red = new Vector4(1, 0, 0, 1);
        public Vector2 child = new Vector2(400, 300);
        public Vector4 border = new Vector4(0, 0, 0, 1f);
        public Vector4 text = new Vector4(1, 1, 1, 1);
        public Vector4 title = new Vector4(1, 1, 1, 1);
        public Vector4 childBgColor = new Vector4(0.0f, 0.0f, 0.0f, 0.8f);
        public Vector4 childBorderColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);

        #endregion

        // bool region
        #region
        static bool isBypassStream = false;
        public bool enableSilent = true;
        bool nameBuggy = false;
        bool healthBuggy = false;
        bool health = false;
        bool bonebuggy = false;
        private bool enableESP = false;
        public bool enableName = false;
        public bool enableBones = false;
        public bool aimbot = true;
        public bool aimOnTeam = false;
        bool show = false;
        public bool logg = false;
        #endregion

        // float region
        #region
        float alpha = 2f;
        float boxThickness = 1f;
        float boneThickness = 4f;
        public float recoilControlStrength = 0.0f;
        public float spreadControlStrength = 0.0f; // 0.0 bedeutet kein Spread, 1.0 ist voller Spread
        public float r = 1.0f;
        public float g = 1.0f;
        public float b = 0.0f;
        float sd2 = 1f;
        public float FOV = 50;
        public float smoothness = 45.0f;
        public float magnetismStrength = 0.1f;
        #endregion

        //int region
        #region
        int LineType = 0;
        int BoxType = 0;
        int CircleType = 0;
        //public int desiredRecoil = 0;
        public int speed = 0;
        public int fov = 105;
        int selectedTab = 0;
        #endregion

        // some stuff
        #region
        private ConcurrentQueue<Entity> entities = new ConcurrentQueue<Entity>();
        private Entity localPlayer = new Entity();
        private readonly object entityLock = new object();
        #endregion

        // Update Color
        #region
        void UpdateColor()
        {
            if (r == 1.0f && g >= 0.0f && b <= 0.0f)
            {
                g += 0.005f;
                b = 0.0f;
            }
            if (r <= 1.0f && g >= 1.0f && b == 0.0f)
            {
                g = 1.0f;
                r -= 0.005f;
            }
            if (r <= 0.0f && g == 1.0f && b >= 0.0f)
            {
                r = 0.0f;
                b += 0.005f;
            }
            if (r == 0.0f && g <= 1.0f && b >= 1.0f)
            {
                b = 1.0f;
                g -= 0.005f;
            }
            if (r >= 0.0f && g <= 0.0f && b == 1.0f)
            {
                g = 0.0f;
                r += 0.005f;
            }
            if (r >= 1.0f && g >= 0.0f && b <= 1.0f)
            {
                r = 1.0f;
                b -= 0.005f;
            }
            Thread.Sleep(1);
        }
        #endregion

        public ImDrawListPtr drawList;
        protected override void Render()
        {
            Hide();
            DrawOverlay(screenSize);
            if (GetAsyncKeyState(0x2D) < 0)
            {
                show = !show;
                Thread.Sleep(150);
            }
            if (show)
            {
                //if (speed > maxSpeed)
                //maxSpeed = speed;
                // ImGUI Menu
                ImGuiStylePtr style = ImGui.GetStyle();
                ImGui.SetNextWindowSize(sd3);
                ImGui.SetNextWindowPos(new Vector2(0, 0), ImGuiCond.Appearing);
                style.Alpha = alpha;
                style.WindowBorderSize = 2f;
                style.FrameRounding = 20f;
                style.WindowRounding = 8f;
                style.SeparatorTextBorderSize = sd2;
                style.Colors[(int)ImGuiCol.Border] = border;
                style.Colors[(int)ImGuiCol.Text] = text;
                style.Colors[(int)ImGuiCol.TitleBgActive] = title;
                style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0, 0, 0, 1f);
                style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
                style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
                style.Colors[(int)ImGuiCol.Tab ] = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
                style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
                style.Colors[(int)ImGuiCol.Button] = new Vector4(0, 0, 0, 1f); ;
                style.Colors[(int)ImGuiCol.Header] = new Vector4(0.5f, 0.5f, 0.5f,1.0f);
                style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.0f, 0.0f, 0.1f, 0.5f);
                style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.0f, 1.0f, 0.0f, 0.5f);
                style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.0f, 0.0f, 0.1f, 1.0f);
                style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.0f, 0.0f, 0.1f, 1.0f);
                style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.0f, 0.0f, 0.1f, 1.0f);
                style.TabRounding = 8f;
                style.PopupRounding = 8f;
                style.GrabRounding = 8f;
                style.ChildRounding = 1f;
                style.DisabledAlpha = 1f;
                style.FrameBorderSize = 0.5f;
                style.ItemSpacing = new Vector2(0.5f, 0.5f);
                ImGui.Begin("CS2 External", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
                /*if (ImGui.BeginTabBar("Options"))

                {
                    UpdateColor();
                    if (ImGui.BeginTabItem("Visuals"))
                    {
                        UpdateColor();
                        ImGui.BeginChild("Visuals", child, true);
                        ImGui.Combo("Box Type", ref BoxType, new string[] { "None", "Normal Box", "Cornered Box", "Filled Box", "Normal Box + Rainbow", "Cornered Box + Rainbow" }, 6);
                        ImGui.Combo("Line Type", ref LineType, new string[] { "None", "Line Bottom", "Line Middle" }, 3);
                        ImGui.Checkbox("Enable name", ref enableName);
                        ImGui.Checkbox("Enable bones", ref enableBones);
                        ImGui.Checkbox("Bones (buggy)", ref bonebuggy);
                        ImGui.Checkbox("Health", ref health);
                        ImGui.Checkbox("Health (buggy)", ref healthBuggy);
                        ImGui.EndChild();
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Aim"))
                    {
                        ImGui.BeginChild("Aim", child, true);
                        ImGui.Checkbox("AimBot", ref aimbot);
                        ImGui.Checkbox("Aim on Team", ref aimOnTeam);
                        ImGui.Combo("Circle type", ref CircleType, new string[] { "Circle (normal)", "Filled Circle (normal)", "Circle (rgb)", "Filled Circle (rgb)" }, 4);
                        ImGui.SliderFloat("Aim Fov", ref FOV, 10, 300);
                        ImGui.SliderFloat("smoothness", ref smoothness, 1.0f, 100.0f);
                        //ImGui.SliderFloat("Magnet", ref magnetismStrength, 0.1f, 1.0f);
                        ImGui.EndChild();
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Misc"))
                    {
                        ImGui.BeginChild("Misc", child, true);


                        ImGui.ColorEdit4("team color", ref teamColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy color", ref enemyColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy bone color", ref boneColor);
                        ImGui.Separator();
                        ImGui.ColorEdit4("team bone color", ref teamColorBone);
                        ImGui.Separator();
                        ImGui.SliderInt("FOV", ref fov, 1, 165);
                        ImGui.Separator();
                        ImGui.SliderFloat("Box Thickness", ref boxThickness, 1f, 10f);
                        ImGui.EndChild();
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Menu"))
                    {
                        ImGui.BeginChild("Colors", child, true);
                        ImGui.ColorEdit4("team color", ref teamColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy color", ref enemyColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy bone color", ref boneColor);
                        ImGui.Separator();
                        ImGui.ColorEdit4("team bone color", ref teamColorBone);
                        ImGui.Separator();
                        ImGui.ColorEdit4("Text Color", ref text);
                        ImGui.Separator();
                        ImGui.ColorEdit4("Title Color", ref title);
                        ImGui.Separator();
                        ImGui.ColorEdit4("Border", ref border);
                        ImGui.Separator();
                        ImGui.SliderFloat("Alpha", ref alpha, 0.2f, 1f);
                        ImGui.Separator();
                        ImGui.Checkbox("Stream proof", ref isBypassStream);
                        ImGui.EndChild();
                        ImGui.EndTabItem();
                    }
                }
                ImGui.EndTabBar();*/
                /*ImGui.BeginChild("SideBar", new Vector2(150, 0), true);

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(20, 20));  // Abstand zwischen den Selectables
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(20, 20)); // Polsterung (Innenabstand) der Selectables
                ImGui.PushStyleColor(ImGuiCol.ChildBg, childBgColor);        // Hintergrundfarbe des Childs
                if (ImGui.Selectable("Visuals", selectedTab == 0)) selectedTab = 0;
                if (ImGui.Selectable("Aim", selectedTab == 1)) selectedTab = 1;
                if (ImGui.Selectable("Misc", selectedTab == 2)) selectedTab = 2;
                if (ImGui.Selectable("Menu", selectedTab == 3)) selectedTab = 3;
                ImGui.PopStyleVar(3);
                ImGui.EndChild();

                ImGui.SameLine(); // Nebeneinander anordnen
                ImGui.BeginChild("Tabs", new Vector2(0, 0), true);

                switch (selectedTab)
                {
                    case 0: // Visuals
                        ImGui.Combo("Box Type", ref BoxType, new string[] { "None", "Normal Box", "Cornered Box", "Filled Box", "Normal Box + Rainbow", "Cornered Box + Rainbow" }, 6);
                        ImGui.Combo("Line Type", ref LineType, new string[] { "None", "Line Bottom", "Line Middle" }, 3);
                        ImGui.Checkbox("Enable name", ref enableName);
                        ImGui.Checkbox("Enable bones", ref enableBones);
                        ImGui.Checkbox("Bones (buggy)", ref bonebuggy);
                        ImGui.Checkbox("Health", ref health);
                        ImGui.Checkbox("Health (buggy)", ref healthBuggy);
                        break;
                    case 1: // Aim
                        ImGui.Checkbox("AimBot", ref aimbot);
                        ImGui.Checkbox("Aim on Team", ref aimOnTeam);
                        ImGui.Combo("Circle type", ref CircleType, new string[] { "Circle (normal)", "Filled Circle (normal)", "Circle (rgb)", "Filled Circle (rgb)" }, 4);
                        ImGui.SliderFloat("Aim Fov", ref FOV, 10, 600);
                        ImGui.SliderFloat("smoothness", ref smoothness, 1.0f, 80.0f);
                        //ImGui.SliderFloat("Magnet", ref magnetismStrength, 0.1f, 1.0f);
                        break;
                    case 2: // Misc
                        ImGui.ColorEdit4("team color", ref teamColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy color", ref enemyColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy bone color", ref boneColor);
                        ImGui.Separator();
                        ImGui.ColorEdit4("team bone color", ref teamColorBone);
                        ImGui.Separator();
                        ImGui.SliderInt("FOV", ref fov, 1, 165);
                        ImGui.Separator();
                        ImGui.SliderFloat("Box Thickness", ref boxThickness, 1f, 10f);
                        break;
                    case 3: // Menu
                        ImGui.ColorEdit4("team color", ref teamColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy color", ref enemyColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy bone color", ref boneColor);
                        ImGui.Separator();
                        ImGui.ColorEdit4("team bone color", ref teamColorBone);
                        ImGui.Separator();
                        ImGui.ColorEdit4("Text Color", ref text);
                        ImGui.Separator();
                        ImGui.ColorEdit4("Title Color", ref title);
                        ImGui.Separator();
                        ImGui.ColorEdit4("Border", ref border);
                        ImGui.Separator();
                        ImGui.SliderFloat("Alpha", ref alpha, 0.2f, 1f);
                        ImGui.Separator();
                        ImGui.Checkbox("Stream proof", ref isBypassStream);
                        break;
                }

                ImGui.EndChild();*/

                if (ImGui.Selectable("Visuals", selectedTab == 0, ImGuiSelectableFlags.None, new Vector2(80, 40))) selectedTab = 0;
                ImGui.SameLine(); // Tabs nebeneinander
                if (ImGui.Selectable("Aim", selectedTab == 1, ImGuiSelectableFlags.None, new Vector2(80, 40))) selectedTab = 1;
                ImGui.SameLine();
                if (ImGui.Selectable("Misc", selectedTab == 2, ImGuiSelectableFlags.None, new Vector2(80, 40))) selectedTab = 2;
                ImGui.SameLine();
                if (ImGui.Selectable("Menu", selectedTab == 3, ImGuiSelectableFlags.None, new Vector2(80, 40))) selectedTab = 3;
                ImGui.SameLine();
                if (ImGui.Selectable("Config", selectedTab == 4, ImGuiSelectableFlags.None, new Vector2(80, 40))) selectedTab = 4;
                
                // Inhalt unterhalb der Tabs anzeigen
                ImGui.BeginChild("TabContent", new Vector2(0, 0));

                switch (selectedTab)
                {
                    case 0: // Visuals
                        ImGui.Combo("Box Type", ref BoxType, new string[] { "None", "Normal Box", "Cornered Box", "Filled Box", "Normal Box + Rainbow", "Cornered Box + Rainbow" }, 6);
                        ImGui.Combo("Line Type", ref LineType, new string[] { "None", "Line Bottom", "Line Middle" }, 3);
                        ImGui.Checkbox("Enable name", ref enableName);
                        ImGui.Checkbox("Enable bones", ref enableBones);
                        ImGui.Checkbox("Bones (buggy)", ref bonebuggy);
                        ImGui.Checkbox("Health", ref health);
                        ImGui.Checkbox("Health (buggy)", ref healthBuggy);
                        break;
                    case 1: // Aim
                        ImGui.Checkbox("AimBot", ref aimbot);
                        ImGui.Checkbox("Aim on Team", ref aimOnTeam);
                        ImGui.Combo("Circle type", ref CircleType, new string[] { "Circle (normal)", "Filled Circle (normal)", "Circle (rgb)", "Filled Circle (rgb)", "None" }, 5);
                        ImGui.SliderFloat("Aim Fov", ref FOV, 5, 200);
                        ImGui.SliderFloat("smoothness", ref smoothness, 1.0f, 80.0f);
                        break;
                    case 2: // Misc
                        ImGui.ColorEdit4("team color", ref teamColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy color", ref enemyColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy bone color", ref boneColor);
                        ImGui.Separator();
                        ImGui.ColorEdit4("team bone color", ref teamColorBone);
                        ImGui.Separator();
                        ImGui.SliderInt("FOV", ref fov, 1, 165);
                        ImGui.Separator();
                        ImGui.SliderFloat("Box Thickness", ref boxThickness, 1f, 10f);
                        break;
                    case 3: // Menu
                        ImGui.ColorEdit4("team color", ref teamColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy color", ref enemyColor, ImGuiColorEditFlags.AlphaBar);
                        ImGui.Separator();
                        ImGui.ColorEdit4("enemy bone color", ref boneColor);
                        ImGui.Separator();
                        ImGui.ColorEdit4("team bone color", ref teamColorBone);
                        ImGui.Separator();
                        ImGui.ColorEdit4("Text Color", ref text);
                        ImGui.Separator();
                        ImGui.ColorEdit4("Title Color", ref title);
                        ImGui.Separator();
                        ImGui.ColorEdit4("Border", ref border);
                        ImGui.Separator();
                        ImGui.SliderFloat("Alpha", ref alpha, 0.2f, 1f);
                        ImGui.Separator();
                        ImGui.Checkbox("Stream proof", ref isBypassStream);
                        break;
                    case 4:
                        if(ImGui.Selectable("Legit"))
                        {
                            enableBones = true;
                            FOV = 20;
                            smoothness = 80.0f;
                            LineType = 2;
                            aimbot = false;
                            BoxType = 2;
                            health = true;
                            CircleType = 5;
                        }
                        if (ImGui.Selectable("Semi Legit"))
                        {
                            enableBones = true;
                            FOV = 30;
                            smoothness = 45f;
                            LineType = 2;
                            aimbot = true;
                            BoxType = 4;
                            health = true;
                            CircleType = 3;
                        }
                        if (ImGui.Selectable("Rage"))
                        {
                            enableBones = true;
                            FOV = 200;
                            smoothness = 10.0f;
                            BoxType = 4;
                            health = true;
                            aimbot= true;
                            isBypassStream = true;
                        }
                        break;

                }

                ImGui.EndChild();


                //ImGui.TextColored(GetSpeedColor(maxSpeed), $"Max speed: {maxSpeed}");
                //ImGui.TextColored(GetSpeedColor(speed), $"Current speed: {speed}");

                /*if (CircleType == 2)
                {
                    Vector4 circleColorRGB = new Vector4(r, g, b, 0.5f);
                    drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColorRGB));
                }
                if (CircleType == 3)
                {
                    Vector4 circleColorRGB = new Vector4(r, g, b, 0.5f);
                    drawList.AddCircleFilled(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColorRGB));
                }
                if (CircleType == 0)
                {
                    drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColor));
                }
                if (CircleType == 1)
                {
                    drawList.AddCircleFilled(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColor));
                }*/
                if (bonebuggy)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            DrawBonesBuggy(entity);
                        }
                    }
                }
                if (health)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            DrawHealthBar(entity);
                        }
                    }
                }

                if (BoxType == 0)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {

                        }
                    }
                }
                if (LineType == 0)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {

                        }
                    }
                }
                if (LineType == 1)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            DrawLineBottom(entity);
                        }
                    }
                }
                if (LineType == 2)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            DrawLineMiddle(entity);
                        }
                    }
                }
                if (BoxType == 1)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            
                            Draw2DBox(entity);
                        }
                    }
                }
                if (BoxType == 2)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {

                            DrawCorneredBox(entity);
                        }
                    }
                }
                if (BoxType == 3)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            DrawFilledBox(entity);
                        }
                    }
                }
                if (BoxType == 4)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            Draw2DBoxBuggy(entity);
                        }
                    }
                }
                if (BoxType == 5)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            DrawCorneredBoxBuggy(entity);
                        }
                    }
                }

                if (enableESP)
                {
                    foreach (var entity in entities)
                    {

                        if (EntityOnScreen(entity))
                        {
                            Draw2DBox(entity);
                        }
                    }
                }
                if (enableName)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            DrawName(entity, 20);
                        }
                    }
                }
                if (nameBuggy)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            DrawNameBuggy(entity, 20);
                        }
                    }
                }
                if (enableBones)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            DrawBones(entity);
                        }
                    }
                }
                if (healthBuggy)
                {
                    foreach (var entity in entities)
                    {
                        if (EntityOnScreen(entity))
                        {
                            DrawHealthBarBuggy(entity);
                        }
                    }
                }

                ImGui.End();
            }
            #region
            if (CircleType == 2)
            {
                Vector4 circleColorRGB = new Vector4(r, g, b, 05f);
                drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColorRGB));
            }
            if (CircleType == 3)
            {
                Vector4 circleColorRGB = new Vector4(r, g, b, 05f);
                drawList.AddCircleFilled(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColorRGB));
            }
            if (CircleType == 0)
            {
                drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColor));
            }
            if (CircleType == 1)
            {
                drawList.AddCircleFilled(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColor));
            }
            if (bonebuggy)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        DrawBonesBuggy(entity);
                    }
                }
            }
            if (health)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        DrawHealthBar(entity);
                    }
                }
            }
            if (LineType == 0)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {

                    }
                }
            }
            if (LineType == 1)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        DrawLineBottom(entity);
                    }
                }
            }
            if (LineType == 2)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        DrawLineMiddle(entity);
                    }
                }
            }
            if (BoxType == 0)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {

                    }
                }
            }
            if (BoxType == 1)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {

                        Draw2DBox(entity);
                    }
                }
            }
            if (BoxType == 2)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {

                        DrawCorneredBox(entity);
                    }
                }
            }
            if (BoxType == 3)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        DrawFilledBox(entity);
                    }
                }
            }
            if (BoxType == 4)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        Draw2DBoxBuggy(entity);
                    }
                }
            }
            if (BoxType == 5)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        DrawCorneredBoxBuggy(entity);
                    }
                }
            }
            if (enableESP)
            {
                foreach (var entity in entities)
                {

                    if (EntityOnScreen(entity))
                    {
                        Draw2DBox(entity);
                    }
                }
            }
            if (enableName)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        DrawName(entity, 20);
                    }
                }
            }
            if (nameBuggy)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        DrawNameBuggy(entity, 20);
                    }
                }
            }
            if (enableBones)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        DrawBones(entity);
                    }
                }
            }
            if (healthBuggy)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        DrawHealthBarBuggy(entity);
                    }
                }
            }
            #endregion
        }

        Vector4 GetSpeedColor(int speed)
        {
            if (speed > 500)
                return red;
            if (speed > 400)
                return yellow;
            if (speed > 300)
                return green;
            return white;
        }

        bool EntityOnScreen(Entity entity)
        {
            if (entity.position2D.X > 0 && entity.position2D.X < screenSize.X && entity.position2D.Y > 0 && entity.position2D.Y < screenSize.Y)
            {
                return true;
            }
            return false;
        }
        private void DrawBones(Entity entity)
        {
            Vector4 boneColor1 = localPlayer.team == entity.team ? teamColorBone : boneColor;
            uint uintColor = ImGui.ColorConvertFloat4ToU32(boneColor1);

            float currentBoneThickness = boneThickness / entity.distance;
            drawList.AddLine(entity.bones2d[1], entity.bones2d[2], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[1], entity.bones2d[3], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[1], entity.bones2d[6], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[3], entity.bones2d[4], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[6], entity.bones2d[7], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[4], entity.bones2d[5], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[7], entity.bones2d[8], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[1], entity.bones2d[0], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[0], entity.bones2d[9], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[0], entity.bones2d[11], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[9], entity.bones2d[10], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[11], entity.bones2d[12], uintColor, currentBoneThickness);
            drawList.AddCircle(entity.bones2d[2], 4 + currentBoneThickness, uintColor);
        }
        private void DrawBonesBuggy(Entity entity)
        {
            uint uintColor = ImGui.ColorConvertFloat4ToU32(new Vector4(r, g, b, 1f));

            float currentBoneThickness = 0.5f;
            drawList.AddLine(entity.bones2d[1], entity.bones2d[2], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[1], entity.bones2d[3], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[1], entity.bones2d[6], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[3], entity.bones2d[4], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[6], entity.bones2d[7], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[4], entity.bones2d[5], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[7], entity.bones2d[8], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[1], entity.bones2d[0], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[0], entity.bones2d[9], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[0], entity.bones2d[11], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[9], entity.bones2d[10], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[11], entity.bones2d[12], uintColor, currentBoneThickness);
            drawList.AddCircle(entity.bones2d[2], 5 + currentBoneThickness, uintColor);
        }

        void DrawHealthBar(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;
            float boxLeft = entity.viewPosition2D.X - entityHeight / 3;
            float boxRight = entity.position2D.X + entityHeight / 3;

            float barPercentWidth = 0.05f;
            float barPixelWidth = barPercentWidth * (boxRight - boxLeft);
            float barHeight = entityHeight * (entity.health / 100f);
            Vector2 barTop = new Vector2(boxLeft - barPixelWidth, entity.position2D.Y - barHeight);
            Vector2 barBottom = new Vector2(boxLeft, entity.position2D.Y);
            Vector4 barColor = new Vector4(0, 1, 0, 1);
            drawList.AddRectFilled(barTop, barBottom, ImGui.ColorConvertFloat4ToU32(barColor));
        }
        void DrawHealthBarBuggy(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;
            float boxLeft = entity.viewPosition2D.X - entityHeight / 3;
            float boxRight = entity.position2D.X + entityHeight / 3;

            float barPercentWidth = 0.05f;
            float barPixelWidth = barPercentWidth * (boxRight -  boxLeft);
            float barHeight = entityHeight * (entity.health / 100f);
            Vector2 barTop = new Vector2(boxLeft - barPixelWidth, entity.position2D.Y - barHeight);
            Vector2 barBottom = new Vector2(boxLeft, entity.position2D.Y);
            drawList.AddRectFilled(barTop, barBottom, ImGui.ColorConvertFloat4ToU32(new Vector4(r, g, b, 1f)));
        }
        private void DrawNameBuggy(Entity entity, int yOffset)   
        {
            if (enableName)
            {
                string entityName = $"{entity.name}";
                Vector2 textSize = ImGui.CalcTextSize(entityName);

                // Textposition anpassen, damit er zentriert ist
                Vector2 textLocation = new Vector2(entity.viewPosition2D.X - textSize.X / 2, entity.viewPosition2D.Y - yOffset);

                // Text zeichnen
                drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(new Vector4(r, g, b, 1f)), entityName);
            }
        }

        private void DrawName(Entity entity, int yOffset)
        {
            if (enableName)
            {
                string entityName = $"{entity.name}";
                Vector2 textSize = ImGui.CalcTextSize(entityName);

                // Textposition anpassen, damit er zentriert ist
                Vector2 textLocation = new Vector2(entity.viewPosition2D.X - textSize.X / 2, entity.viewPosition2D.Y - yOffset);

                // Text zeichnen
                drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColor), entityName);
            }
        }
        private void DrawCorneredBoxBuggy(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            // Berechnung der Eckpunkte
            Vector2 rectTopLeft = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y);
            Vector2 rectTopRight = new Vector2(entity.viewPosition2D.X + entityHeight / 3, entity.viewPosition2D.Y);
            Vector2 rectBottomLeft = new Vector2(entity.position2D.X - entityHeight / 3, entity.position2D.Y);
            Vector2 rectBottomRight = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);

            // Wähle die Farbe basierend auf dem Team
            Vector4 boxColor = new Vector4(r, g, b, 1.0f);

            // Zeichne die Ecken
            float lineLength = entityHeight / 5; // Länge der Eckenlinien
            var drawList = ImGui.GetBackgroundDrawList();

            // Obere linke Ecke
            drawList.AddLine(rectTopLeft, new Vector2(rectTopLeft.X, rectTopLeft.Y + lineLength), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);
            drawList.AddLine(rectTopLeft, new Vector2(rectTopLeft.X + lineLength, rectTopLeft.Y), ImGui.ColorConvertFloat4ToU32(boxColor));

            // Obere rechte Ecke
            drawList.AddLine(rectTopRight, new Vector2(rectTopRight.X, rectTopRight.Y + lineLength), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);
            drawList.AddLine(rectTopRight, new Vector2(rectTopRight.X - lineLength, rectTopRight.Y), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);

            // Untere linke Ecke
            drawList.AddLine(rectBottomLeft, new Vector2(rectBottomLeft.X, rectBottomLeft.Y - lineLength), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);
            drawList.AddLine(rectBottomLeft, new Vector2(rectBottomLeft.X + lineLength, rectBottomLeft.Y), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);

            // Untere rechte Ecke
            drawList.AddLine(rectBottomRight, new Vector2(rectBottomRight.X, rectBottomRight.Y - lineLength), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);
            drawList.AddLine(rectBottomRight, new Vector2(rectBottomRight.X - lineLength, rectBottomRight.Y), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);
        }

        private void DrawCorneredBox(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            // Berechnung der Eckpunkte
            Vector2 rectTopLeft = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y);
            Vector2 rectTopRight = new Vector2(entity.viewPosition2D.X + entityHeight / 3, entity.viewPosition2D.Y);
            Vector2 rectBottomLeft = new Vector2(entity.position2D.X - entityHeight / 3, entity.position2D.Y);
            Vector2 rectBottomRight = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);

            // Wähle die Farbe basierend auf dem Team
            Vector4 boxColor = localPlayer.team == entity.team ? teamColor : enemyColor;

            // Zeichne die Ecken
            float lineLength = entityHeight / 5; // Länge der Eckenlinien
            var drawList = ImGui.GetBackgroundDrawList();

            // Obere linke Ecke
            drawList.AddLine(rectTopLeft, new Vector2(rectTopLeft.X, rectTopLeft.Y + lineLength), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);
            drawList.AddLine(rectTopLeft, new Vector2(rectTopLeft.X + lineLength, rectTopLeft.Y), ImGui.ColorConvertFloat4ToU32(boxColor));

            // Obere rechte Ecke
            drawList.AddLine(rectTopRight, new Vector2(rectTopRight.X, rectTopRight.Y + lineLength), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);
            drawList.AddLine(rectTopRight, new Vector2(rectTopRight.X - lineLength, rectTopRight.Y), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);

            // Untere linke Ecke
            drawList.AddLine(rectBottomLeft, new Vector2(rectBottomLeft.X, rectBottomLeft.Y - lineLength), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);
            drawList.AddLine(rectBottomLeft, new Vector2(rectBottomLeft.X + lineLength, rectBottomLeft.Y), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);

            // Untere rechte Ecke
            drawList.AddLine(rectBottomRight, new Vector2(rectBottomRight.X, rectBottomRight.Y - lineLength), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);
            drawList.AddLine(rectBottomRight, new Vector2(rectBottomRight.X - lineLength, rectBottomRight.Y), ImGui.ColorConvertFloat4ToU32(boxColor), boxThickness);
        }


        private void Draw2DBoxBuggy(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y);

            Vector2 rectTopBottom = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);

            Vector4 boxColor = new Vector4(r, g, b, 1f);

            drawList.AddRect(rectTop, rectTopBottom, ImGui.ColorConvertFloat4ToU32(boxColor), 2f, ImDrawFlags.None, boxThickness);
        }

        private void Draw2DBox(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y);

            Vector2 rectTopBottom = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);

            Vector4 boxColor = localPlayer.team == entity.team ? teamColor : enemyColor;

            drawList.AddRect(rectTop, rectTopBottom, ImGui.ColorConvertFloat4ToU32(boxColor), 0f, ImDrawFlags.None, boxThickness);
        }

        private void DrawFilledBox(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y);

            Vector2 rectTopBottom = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);

            Vector4 boxColor = localPlayer.team == entity.team ? teamColor : enemyColor;

            drawList.AddRectFilled(rectTop, rectTopBottom, ImGui.ColorConvertFloat4ToU32(boxColor), 1f, ImDrawFlags.None);
        }

        private void DrawLineBottom(Entity entity)
        {
            Vector4 lineColor = localPlayer.team == entity.team ? teamColor : enemyColor;

            drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y), entity.position2D, ImGui.ColorConvertFloat4ToU32(lineColor));
        }
        private void DrawLineMiddle(Entity entity)
        {
            Vector4 lineColor = localPlayer.team == entity.team ? teamColor : enemyColor;

            drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y / 2), entity.position2D, ImGui.ColorConvertFloat4ToU32(lineColor));
        }


        private void DrawLineTop(Entity entity)
        {
            Vector4 lineColor = localPlayer.team == entity.team ? teamColor : enemyColor;

            drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y * 2), entity.position2D, ImGui.ColorConvertFloat4ToU32(lineColor));
        }
        public void UpdateEntities(IEnumerable<Entity> newEntities)
        {
            entities = new ConcurrentQueue<Entity>(newEntities);
        }
        public void UpdateLocalPlayer(Entity newEntity)
        {
            lock (entityLock)
            {
                localPlayer = newEntity;
            }
        }
        public Entity GetLocalPlayer()
        {
            lock (entityLock)
            {
                return localPlayer;
            }
        }
        void DrawOverlay(Vector2 screenSize)
        {
            ImGui.SetNextWindowSize(screenSize);
            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse
                );
        }
        void SetDarkStyle(ImGuiStylePtr style)
        {
            style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.1f, 0.1f, 0.1f, 1.0f); // Dunkelgrau
                                                                                        // Weitere Farbanpassungen für Dark Mode ...
        }

        void SetLightStyle(ImGuiStylePtr style)
        {
            style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); // Weiß
                                                                                        // Weitere Farbanpassungen für Light Mode ...
        }

    }
}
