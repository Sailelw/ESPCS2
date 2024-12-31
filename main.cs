using ESPCS2;
using HtmlAgilityPack;
using Swed64;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
    // Hier initialisierst du alle nötigen Objekte wie Renderer, Swed und Variablen
    Swed swed = new Swed("cs2");
    Renderer renderer = new Renderer();
    List<Entity> entities = new List<Entity>();
    Entity localPlayer = new Entity();

    int dwEntityList = 0x1A146E8;
    int dwViewMatrix = 0x1A7F610;
    int dwLocalPlayerPawn = 0x1868CC8;
    int dwViewAngles = 0x1A89710;

    int m_vOldOrigin = 0x1324;
    int m_iTeamNum = 0x3E3;
    int m_lifeState = 0x348;
    int m_hPlayerPawn = 0x80C;
    int m_vecViewOffset = 0xCB0;
    int m_pCameraServices = 0x11E0;
    int m_iFOV = 0x210;
    int m_bIsScoped = 0x23D0;
    int m_vecAbsVelocity = 0x3F0;
    int m_iszPlayerName = 0x660;
    int m_modelState = 0x170;
    int m_pGameSceneNode = 0x328;
    int m_iHealth = 0x344;

    const int HOTKEY = 0x06;

        // Hier startet das Programm, wenn der Key korrekt war

        IntPtr client = swed.GetModuleBase("client.dll");

        // Starte den Renderer in einem neuen Thread
        Thread renderThread = new Thread(new ThreadStart(renderer.Start().Wait));
        renderThread.Start();

        Vector2 screenSize = renderer.screenSize;

        // Spiel-Logik startet hier
        while (true)
        {
            entities.Clear();

            IntPtr entityList = swed.ReadPointer(client, dwEntityList);

            IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

            IntPtr localPlayerPawn = swed.ReadPointer(client, dwLocalPlayerPawn);

            Vector3 velocity = swed.ReadVec(localPlayerPawn, m_vecAbsVelocity);

            int speed = (int)Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y + velocity.Z * velocity.Z);

            localPlayer.team = swed.ReadInt(localPlayerPawn, m_iTeamNum);
            localPlayer.pawnAddress = swed.ReadPointer(client, dwLocalPlayerPawn);
            localPlayer.origin = swed.ReadVec(localPlayer.pawnAddress, m_vOldOrigin);
            localPlayer.view = swed.ReadVec(localPlayer.pawnAddress, m_vecViewOffset);

            renderer.speed = speed;
            for (int i = 0; i < 64; i++)
            {
                IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);

                if (currentController == IntPtr.Zero) continue;

                int pawnHandle = swed.ReadInt(currentController, m_hPlayerPawn);
                if (pawnHandle == 0) continue;

                IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
                if (listEntry2 == IntPtr.Zero) continue;

                IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FFF));
                if (currentPawn == localPlayer.pawnAddress) continue;

                int lifeState = swed.ReadInt(currentPawn, m_lifeState);
                if (lifeState != 256) continue;

                int health = swed.ReadInt(currentPawn, m_iHealth);
                int team = swed.ReadInt(currentPawn, m_iTeamNum);
                //uint lifeState = swed.ReadUInt(currentPawn, m_lifeState);

                float[] viewMatrix = swed.ReadMatrix(client + dwViewMatrix);

                if (team == localPlayer.team && !renderer.aimOnTeam)
                    continue;
                IntPtr sceneNode = swed.ReadPointer(currentPawn, m_pGameSceneNode);
                IntPtr boneMatrix = swed.ReadPointer(sceneNode, m_modelState + 0x80);
                Entity entity = new Entity();

                entity.pawnAddress = currentPawn;
                entity.controllerAdress = currentController;
                entity.health = health;
                entity.lifestate = lifeState;
                entity.origin = swed.ReadVec(currentPawn, m_vOldOrigin);
                entity.view = swed.ReadVec(currentPawn, m_vecViewOffset);
                entity.name = swed.ReadString(currentController, m_iszPlayerName, 16).Split("\0")[0];
                entity.team = swed.ReadInt(currentPawn, m_iTeamNum);
                entity.position = swed.ReadVec(currentPawn, m_vOldOrigin);
                entity.viewOffset = swed.ReadVec(currentPawn, m_vecViewOffset);
                entity.position2D = Calculate.WorldToScreen(viewMatrix, entity.position, screenSize);
                entity.viewPosition2D = Calculate.WorldToScreen(viewMatrix, Vector3.Add(entity.position, entity.viewOffset), screenSize);
                entity.distance = Vector3.Distance(entity.origin, localPlayer.origin);
                entity.bones = Calculate.ReadBones(boneMatrix, swed);
                entity.bones2d = Calculate.ReadBones2d(entity.bones, viewMatrix, screenSize);
                entity.head = swed.ReadVec(boneMatrix, 6 * 32);
                entity.head2d = Calculate.WorldToScreen(viewMatrix, entity.head, screenSize);

                entity.pixelDistance = Vector2.Distance(entity.head2d, new Vector2(screenSize.X /2, screenSize.Y /2));
                entities.Add(entity);

                Console.ForegroundColor = ConsoleColor.Green;

                if (team != localPlayer.team)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                }
                Console.ResetColor();
            }
            /*entities = entities.OrderBy(o => o.pixelDistance).ToList();
            if (entities.Count > 0 && GetAsyncKeyState(HOTKEY) < 0 && renderer.aimbot)
            {
                Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
                Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

                if (entities[0].pixelDistance < renderer.FOV)
                {
                    Vector2 newAngles = Calculate.CalculateAngles(playerView, entities[0].head);
                    Vector3 newAnglesVec3 = new Vector3(newAngles.Y, newAngles.X, 0.0f);
                    swed.WriteVec(client, dwViewAngles, newAnglesVec3);
                }
            } */

            entities = entities.OrderBy(o => o.pixelDistance).ToList();

            if (entities.Count > 0 && GetAsyncKeyState(HOTKEY) < 0 && renderer.aimbot)
            {
                Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
                Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

                if (entities[0].pixelDistance < renderer.FOV)
                {
                    // Berechne Zielwinkel
                    Vector2 targetAngles = Calculate.CalculateAngles(playerView, entities[0].head);
                    Vector3 targetAnglesVec3 = new Vector3(targetAngles.Y, targetAngles.X, 0.0f);

                    // Lese die aktuellen Blickwinkel
                    Vector3 currentAngles = swed.ReadVec(client, dwViewAngles);

                    // Interpolation für Smoothness
                    Vector3 smoothedAngles = new Vector3(
                        currentAngles.X + (targetAnglesVec3.X - currentAngles.X) / renderer.smoothness,
                        currentAngles.Y + (targetAnglesVec3.Y - currentAngles.Y) / renderer.smoothness,
                        0.0f // Keine Anpassung am Z-Winkel notwendig
                    );

                    // Setze die neuen Blickwinkel
                    swed.WriteVec(client, dwViewAngles, smoothedAngles);
                }
            }
            
            /*entities = entities.OrderBy(o => o.pixelDistance).ToList();

            if (entities.Count > 0 && GetAsyncKeyState(HOTKEY) < 0 && renderer.aimbot)
            {
                Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
                Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

                if (entities[0].pixelDistance < renderer.FOV)
                {
                    // Zielwinkel berechnen
                    Vector2 targetAngles = Calculate.CalculateAngles(playerView, entities[0].head);
                    Vector3 targetAnglesVec3 = new Vector3(targetAngles.Y, targetAngles.X, 0.0f);

                    // Aktuelle Winkel lesen
                    Vector3 currentAngles = swed.ReadVec(client, dwViewAngles);

                    // Magnetischer Effekt: Berechne den Unterschied zwischen Ziel- und aktuellen Winkeln
                    Vector3 angleDelta = new Vector3(
                        targetAnglesVec3.X - currentAngles.X,
                        targetAnglesVec3.Y - currentAngles.Y,
                        0.0f
                    );

                    // Magnetismus anwenden: Ziehe die Blickrichtung in Richtung des Ziels
                    Vector3 magnetizedAngles = new Vector3(
                        currentAngles.X + angleDelta.X * renderer.magnetismStrength,
                        currentAngles.Y + angleDelta.Y * renderer.magnetismStrength,
                        0.0f
                    );

                    // Jetzt Smoothness anwenden: Interpolieren zwischen den aktuellen Blickwinkeln und den magnetisierten Blickwinkeln
                    Vector3 smoothedAngles = new Vector3(
                        currentAngles.X + (magnetizedAngles.X - currentAngles.X) / renderer.smoothness,
                        currentAngles.Y + (magnetizedAngles.Y - currentAngles.Y) / renderer.smoothness,
                        0.0f // Keine Anpassung am Z-Winkel notwendig
                    );

                    // Setze die neuen Blickwinkel
                    swed.WriteVec(client, dwViewAngles, smoothedAngles);
                }
            }*/
            renderer.UpdateLocalPlayer(localPlayer);
            renderer.UpdateEntities(entities);
            uint desiredFov = (uint)renderer.fov;


            IntPtr cameraServices = swed.ReadPointer(localPlayerPawn, m_pCameraServices);

            uint currentFov = swed.ReadUInt(cameraServices + m_iFOV);

            bool isScoped = swed.ReadBool(localPlayerPawn, m_bIsScoped);

            if (currentFov != desiredFov)
            {
                swed.WriteUInt(cameraServices + m_iFOV, desiredFov);
            }
        }
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);
