#include <stdio.h>
#include <stdlib.h>
#include "raylib.h"

// Window settings
const char *title = "ParticleLife";
const int screenWidth = 1280;
const int screenHeight = 800;
const int fps = 144;

// Structs
typedef struct Particle
{
    Vector2 position;
    Vector2 velocity;
    Color color;
    float radius;
} Particle;

// Game settings
const int maxParticles = 1000;
Particle *particles;

// Function prototypes
void initWindow(int screenWidth, int screenHeight, const char *title, int fps);
void gameLoop();

// Function prototypes
void initWindow(int screenWidth, int screenHeight, const char *title, int fps);
void gameLoop();

//----------------------------------------------------------------------------------
// Main entry point
//----------------------------------------------------------------------------------
int main()
{
    // Init window
    initWindow(screenWidth, screenHeight, title, fps);

    // Game loop
    while (!WindowShouldClose())
    {
        BeginDrawing();
        ClearBackground(YELLOW);
        gameLoop();
        DrawFPS(10, 10);
        EndDrawing();
    }

    CloseWindow();
    return 0;
}

// Initialize the Window
void initWindow(int screenWidth, int screenHeight, const char *title, int fps)
{
    printf("Initializing window...\n");
    InitWindow(screenWidth, screenHeight, title);
    SetTargetFPS(fps);

    // Initialize particles
    particles = malloc(maxParticles * sizeof(Particle));
    for (int i = 0; i < maxParticles; i++)
    {
        particles[i].position = (Vector2){GetRandomValue(0, screenWidth), GetRandomValue(0, screenHeight)};
        particles[i].velocity = (Vector2){GetRandomValue(-10, 10), GetRandomValue(-10, 10)};
        particles[i].color = (Color){GetRandomValue(0, 255), GetRandomValue(0, 255), GetRandomValue(0, 255), 255};
        particles[i].radius = GetRandomValue(1, 10);
    }
}

// Game loop
void gameLoop()
{
    // Draw particles
    for (int i = 0; i < maxParticles; i++)
    {
        DrawCircleV(particles[i].position, particles[i].radius, particles[i].color);
    }
}