let spawnPoint = $.subNode("PrefabSpawnPoint");

$.onInteract(() => { 
  const id = new WorldItemTemplateId("PrefabSample03");
  let pos = spawnPoint.getGlobalPosition();
  let rot = spawnPoint.getGlobalRotation();

  $.createItem(id, pos, rot);
});
