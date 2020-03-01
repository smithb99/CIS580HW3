﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

// TODO: replace these with the processor input and output types.

using TInput = ContentPipelineExtension.TilesetContent;
using TOutput = ContentPipelineExtension.TilesetContent;

namespace ContentPipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "Tileset Processor - Tiled")]
    class TilesetProcessor : ContentProcessor<TInput, TOutput>
    {
        /// <summary>
        /// Processes the raw .tsx XML and creates a TilesetContent
        /// for use in an XNA framework game
        /// </summary>
        /// <param name="input">The XML string</param>
        /// <param name="context">The pipeline context</param>
        /// <returns>A TilesetContent instance corresponding to the tsx input</returns>
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            // The image is a separate file - we need to create an external reference to represent it
            // Then we can load that file through the content pipeline and embed it into the TilesetContent
            ExternalReference<TextureContent> externalRef = new ExternalReference<TextureContent>(input.ImageFilename);
            OpaqueDataDictionary options = new OpaqueDataDictionary();
            if (input.ImageColorKey != null) options.Add("ColorKeyColor", input.ImageColorKey);
            input.Texture = context.BuildAndLoadAsset<TextureContent, TextureContent>(externalRef, "TextureProcessor", options, "TextureImporter");

            // Create the Tiles array
            input.Tiles = new TileContent[input.TileCount];

            // Run the logic to generate the individual tile source rectangles
            for (int i = 0; i < input.TileCount; i++)
            {
                var source = new Rectangle(
                        (i % input.Columns) * (input.TileWidth + input.Spacing) + input.Margin, // x coordinate 
                        (i / input.Columns) * (input.TileHeight + input.Spacing) + input.Margin, // y coordinate
                        input.TileWidth,
                        input.TileHeight
                        );
                input.Tiles[i] = new TileContent(source);
            }

            // The tileset has been processed
            return input;
        }
    }
}