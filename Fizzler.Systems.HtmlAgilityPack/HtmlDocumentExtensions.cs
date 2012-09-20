using System.Collections.Generic;

namespace Fizzler.Systems.HtmlAgilityPack
{
    #region Imports

    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using global::HtmlAgilityPack;

    #endregion

    public static class HtmlDocumentExtensions
    {
        private static Dictionary<string, HtmlElementFlag> _defaultElementFlags;

        // TODO Think of a better name than LoadHtml2
        /// <summary>
        /// Same as <see cref="HtmlDocument.LoadHtml" /> but without the FORM nesting
        /// problem outlined in <a href="http://code.google.com/p/fizzler/issues/detail?id=24">issue #24</a>.
        /// </summary>

        public static void LoadHtml2(this HtmlDocument document, string html)
        {
            if (document == null) throw new ArgumentNullException("document");
            document.LoadHtmlWithElementFlags(html, DefaultElementFlags);
        }

        // TODO Think of a better name than LoadHtml2
        /// <summary>
        /// Same as <see cref="HtmlDocument.Load" /> but without the FORM nesting
        /// problem outlined in <a href="http://code.google.com/p/fizzler/issues/detail?id=24">issue #24</a>.
        /// </summary>

        public static void Load2(this HtmlDocument document, string path)
        {
            if (document == null) throw new ArgumentNullException("document");
            document.LoadWithElementFlags(path, DefaultElementFlags);
        }

        /// <summary>
        /// Parses the HTML and loads the document model using supplied
        /// per-element handling options.
        /// </summary>
        /// <remarks>
        /// The behavior of this method is not guaranteed to be thread-safe 
        /// and is primarily a hack around <see cref="HtmlNode.ElementsFlags"/> 
        /// being static.
        /// </remarks>

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void LoadHtmlWithElementFlags(this HtmlDocument document, string html, Dictionary<string, HtmlElementFlag> flags)
        {
            if (document == null) throw new ArgumentNullException("document");
            LoadWithElementFlags(flags, () => document.LoadHtml(html));
        }

        /// <summary>
        /// Parses the HTML and loads the document model using supplied
        /// per-element handling options.
        /// </summary>
        /// <remarks>
        /// The behavior of this method is not guaranteed to be thread-safe 
        /// and is primarily a hack around <see cref="HtmlNode.ElementsFlags"/> 
        /// being static.
        /// </remarks>

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void LoadWithElementFlags(this HtmlDocument document, string path, Dictionary<string, HtmlElementFlag> flags)
        {
            if (document == null) throw new ArgumentNullException("document");
            LoadWithElementFlags(flags, () => document.Load(path));
        }

        private delegate void LoadHandler();

        private static void LoadWithElementFlags(Dictionary<string, HtmlElementFlag> flags, LoadHandler loader)
        {
            var oldFlags = HtmlNode.ElementsFlags;
            try
            {
                if (flags != null)
                    HtmlNode.ElementsFlags = flags;
                loader();
            }
            finally
            {
                HtmlNode.ElementsFlags = oldFlags;
            }
        }

        private static Dictionary<string,HtmlElementFlag> DefaultElementFlags
        {
            get
            {
                if(_defaultElementFlags == null)
                {
                    var flags = new Dictionary<string, HtmlElementFlag>(HtmlNode.ElementsFlags);
                    HtmlElementFlag result;
                    flags.TryGetValue("form", out result);
                    flags["form"] = result | HtmlElementFlag.Closed;
                    _defaultElementFlags = flags;
                }

                return _defaultElementFlags;
            }
        }
    }
}
