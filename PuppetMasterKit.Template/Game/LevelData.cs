using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using PuppetMasterKit.AI;

namespace PuppetMasterKit.Template.Game
{
  public class LevelData
  {
    public Obstacle[] Obstacles { get; set; }

    /// <summary>
    /// Load the specified resourceName.
    /// </summary>
    /// <returns>The load.</returns>
    /// <param name="resourceName">Resource name.</param>
    public static LevelData Load(string resourceName)
    {
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
      using (var resource = new StreamReader(stream))
      {
        var json = resource.ReadToEnd();
        var data = JsonConvert.DeserializeObject<LevelData>(json, new JsonSerializerSettings {
          TypeNameHandling = TypeNameHandling.Auto
        });
        return data;
      }
    }

    /// <summary>
    /// Save the specified resourceName.
    /// </summary>
    /// <returns>The save.</returns>
    /// <param name="resourceName">Resource name.</param>
    public string Save(string resourceName)
    {
      return JsonConvert.SerializeObject(this, new JsonSerializerSettings {
        TypeNameHandling = TypeNameHandling.Auto
      });
    }
  }
}
