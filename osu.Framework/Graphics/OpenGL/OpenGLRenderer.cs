// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.OpenGL.Batches;
using osu.Framework.Graphics.OpenGL.Buffers;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics.ES30;

namespace osu.Framework.Graphics.OpenGL
{
    public class OpenGLRenderer : IRenderer
    {
        public IFrameBuffer CreateFrameBuffer(RenderBufferFormat[]? renderBufferFormats = null, TextureFilteringMode filteringMode = TextureFilteringMode.Linear)
        {
            All glFilteringMode;
            RenderbufferInternalFormat[]? glFormats = null;

            switch (filteringMode)
            {
                case TextureFilteringMode.Linear:
                    glFilteringMode = All.Linear;
                    break;

                case TextureFilteringMode.Nearest:
                    glFilteringMode = All.Nearest;
                    break;

                default:
                    throw new ArgumentException($"Unsupported filtering mode: {filteringMode}", nameof(filteringMode));
            }

            if (renderBufferFormats != null)
            {
                glFormats = new RenderbufferInternalFormat[renderBufferFormats.Length];

                for (int i = 0; i < renderBufferFormats.Length; i++)
                {
                    switch (renderBufferFormats[i])
                    {
                        case RenderBufferFormat.D16:
                            glFormats[i] = RenderbufferInternalFormat.DepthComponent16;
                            break;

                        default:
                            throw new ArgumentException($"Unsupported render buffer format: {renderBufferFormats[i]}", nameof(renderBufferFormats));
                    }
                }
            }

            return new FrameBuffer(glFormats, glFilteringMode);
        }

        public IVertexBatch<TVertex> CreateLinearBatch<TVertex>(int size, int maxBuffers, PrimitiveTopology topology) where TVertex : unmanaged, IEquatable<TVertex>, IVertex
            => new LinearBatch<TVertex>(size, maxBuffers, topology);

        public IVertexBatch<TVertex> CreateQuadBatch<TVertex>(int size, int maxBuffers) where TVertex : unmanaged, IEquatable<TVertex>, IVertex
            => new QuadBatch<TVertex>(size, maxBuffers);
    }
}