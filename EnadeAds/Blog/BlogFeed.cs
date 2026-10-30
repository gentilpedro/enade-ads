using System.Globalization;
using System.Text;
using System.Xml;

namespace EnadeAds.Blog;

public static class BlogFeed
{
    public const string Path = "/blog/feed.xml";

    /// <summary>RSS 2.0 com os posts publicados, para acompanhar o blog fora da plataforma.</summary>
    public static IEndpointRouteBuilder MapBlogFeed(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(Path, async (HttpContext context, BlogService blog, CancellationToken cancellationToken) =>
        {
            var posts = await blog.FeedAsync(cancellationToken: cancellationToken);
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}";

            using var buffer = new MemoryStream();
            // O XmlWriter escapa todo o texto: título e resumo do professor não quebram o XML.
            await using (var xml = XmlWriter.Create(buffer, new XmlWriterSettings { Async = true, Encoding = new UTF8Encoding(false), Indent = true }))
            {
                await xml.WriteStartDocumentAsync();
                xml.WriteStartElement("rss");
                xml.WriteAttributeString("version", "2.0");
                xml.WriteStartElement("channel");
                xml.WriteElementString("title", "Rumo – avisos oficiais");
                xml.WriteElementString("link", $"{baseUrl}/blog");
                xml.WriteElementString("description", "Datas, editais, portarias e dicas de estudo para o ENEM, o ENADE e outras provas.");
                xml.WriteElementString("language", "pt-BR");

                foreach (var post in posts)
                {
                    var link = $"{baseUrl}/blog/{post.Slug}";
                    xml.WriteStartElement("item");
                    xml.WriteElementString("title", post.Title);
                    xml.WriteElementString("link", link);
                    xml.WriteElementString("guid", link);
                    xml.WriteElementString("description", post.Summary);
                    xml.WriteElementString("category", post.Category.Name());
                    xml.WriteElementString("pubDate", post.PublishedAt!.Value.ToString("r", CultureInfo.InvariantCulture));
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
                xml.WriteEndElement();
                await xml.WriteEndDocumentAsync();
            }

            return Results.Bytes(buffer.ToArray(), "application/rss+xml; charset=utf-8");
        });

        return endpoints;
    }
}
