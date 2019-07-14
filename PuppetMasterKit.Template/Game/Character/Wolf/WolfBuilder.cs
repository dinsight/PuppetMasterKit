﻿using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using LightInject;
using PuppetMasterKit.AI.Rules;

namespace PuppetMasterKit.Template.Game.Character.Wolf
{
  public static class WolfBuilder
  {
    private static string CharacterName = "wolf";
    /// <summary>
    /// Build the specified componentSystem and flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="componentSystem">Component system.</param>
    public static Entity Build(ComponentSystem componentSystem, Polygon boundaries)
    {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();

      var entity = EntityBuilder.Build()
        .With(componentSystem,
              new RuleSystemComponent<FlightMap, WolfHandlers>(WolfRulesBuilder.Build(flightMap), new WolfHandlers()),
              new StateComponent<WolfStates>(WolfStates.idle),
              new SpriteComponent(CharacterName, new Size(50, 50)),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(7, 3, 1, 15),
              new CommandComponent(WolfHandlers.OnTouched, WolfHandlers.OnMoveToPoint),
              new CollisionComponent((e) => flightMap.GetAdjacentEntities(e, p => p.Name == "rabbit"), WolfHandlers.WolfMeetsPrey, 30),
              new Agent())
        .WithName(CharacterName)
        .GetEntity();
      
      entity.GetComponent<Agent>()
        .Add(new GoalToWander())
        .Add(new GoalToCohereWith(x => flightMap.GetAdjacentEntities(entity, p => p.Name == CharacterName), 150), 0.0005f)
        .Add(new GoalToSeparateFrom(x => flightMap.GetAdjacentEntities(entity, p => p.Name == CharacterName), 25), 0.1f)
        .Add(new ConstraintToStayWithin(boundaries))
        .Add(new GoalToAvoidObstacles(x=> ((GameFlightMap)flightMap).GetObstacles(entity)));
      
      return entity;
    }
  }
}
