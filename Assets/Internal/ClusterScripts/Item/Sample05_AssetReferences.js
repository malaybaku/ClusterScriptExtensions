// @field(AudioClip)
const myAudio = $.audio("");

// @field(HumanoidAnimation)
const myMotion = $.humanoidAnimation("");

// @field(WorldItem)
const otherItemInstance = $.worldItemReference("");

// @field(WorldItemTemplate)
const creatableItem = new WorldItemTemplateId("");

// @field(Material)
const myRendererMaterial = $.material("");


$.onInteract(() => { 
  $.log("try play audio...");
  myAudio.play();
});
