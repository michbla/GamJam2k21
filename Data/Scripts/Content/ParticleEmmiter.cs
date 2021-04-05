using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace GamJam2k21
{
    public class Particle
    {
        public Vector2 position;
        public Vector2 velocity;
        public Vector4 color;
        public float life;

        public Particle()
        {
            position = new Vector2(0.0f);
            velocity = new Vector2(0.0f);
            color = new Vector4(1.0f);
            life = 0.0f;
        }

    }

    public class ParticleEmmiter
    {
        public ParticleEmmiter(Shader sha, Texture tex, int am)
        {
            shader = sha;
            texture = tex;
            amount = am;
            Init();
        }

        public void SpawnParticles(Vector2 pos, int newParticles, Vector2 offset, Vector3 color, Vector2 vel, Vector2 velStr, bool Y, bool X, bool randPos)
        {
            for (var i = 0; i < newParticles; i++)
            {
                if (FirstUnusedParticle() != -1)
                    RespawnParticle(particles[i], pos, offset, color, vel, velStr, Y, X, randPos);
            }
        }

        public void Update(float dt)
        {
            for (int i = 0; i < amount; i++)
            {
                Particle p = particles[i];
                p.life -= dt;
                if (p.life > 0.0f)
                {
                    p.position += p.velocity * dt;
                    p.color.W -= dt * 2.5f;
                }
            }
        }
        public void Draw(Vector2 viewPos)
        {
            shader.Use();
            foreach (var particle in particles)
            {
                if (particle.life > 0.0f)
                {
                    shader.SetVector2("offset", particle.position);
                    shader.SetVector4("color", particle.color);
                    shader.SetMatrix4("view", Matrix4.CreateTranslation(-viewPos.X, -viewPos.Y, 0.0f));
                    texture.Use(TextureUnit.Texture0);
                    GL.BindVertexArray(vao);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                    GL.BindVertexArray(0);
                }
            }
        }
        private List<Particle> particles;
        private int amount;
        private Shader shader;
        private Texture texture;
        private int vao;

        private void Init()
        {
            particles = new List<Particle>();
            int vbo;
            float[] particleQuad =
            {
            //pos       //tex
            0.0f, 0.01f, 0.0f, 0.0f, //Lewy dolny
            0.01f, 0.0f, 0.01f, 0.01f,
            0.0f, 0.0f, 0.0f, 0.01f,

            0.0f, 0.01f, 0.0f, 0.0f, //Prawy gorny
            0.01f, 0.01f, 0.01f, 0.0f,
            0.01f, 0.0f, 0.01f, 0.01f
            };
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, particleQuad.Length * sizeof(float), particleQuad, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(vao);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            for (var i = 0; i < amount; i++)
            {
                particles.Add(new Particle());
            }
        }
        private int lastUsedParticle = 0;
        private int FirstUnusedParticle()
        {
            for (var i = lastUsedParticle; i < amount; i++)
            {
                if (particles[i].life <= 0.0f)
                {
                    lastUsedParticle = i;
                    return i;
                }
            }
            for (var i = 0; i < lastUsedParticle; i++)
            {
                if (particles[i].life <= 0.0f)
                {
                    lastUsedParticle = i;
                    return i;
                }
            }
            lastUsedParticle = 0;
            return -1;
        }
        private void RespawnParticle(Particle particle, Vector2 pos, Vector2 offset, Vector3 color, Vector2 vel, Vector2 velStr, bool Y, bool X, bool randPos)
        {
            var random = new Random();
            float randX = ((random.Next() % 100) - 50) / 150.0f;
            float randY = (random.Next() % 100 - 50) / 150.0f;
            if (randPos)
                particle.position = (pos.X + randX + offset.X, pos.Y + randY + offset.Y);
            else
                particle.position = (pos.X + offset.X, pos.Y + offset.Y);
            particle.color = (color.X, color.Y, color.Z, 1.0f);
            particle.life = 0.3f;
            float velY;
            float velX;
            if (Y)
                velY = (vel.Y + randY) * velStr.Y;
            else
                velY = vel.Y * velStr.Y;

            if (X)
                velX = (vel.X + randX) * velStr.X;
            else
                velX = vel.X * velStr.X;

            particle.velocity = (velX, velY);
        }
    }
}
