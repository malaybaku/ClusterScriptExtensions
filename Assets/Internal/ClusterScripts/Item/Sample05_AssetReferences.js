// @field(AudioClip)
const myAudio = $.audio("");

// @field(HumanoidAnimation)
const myMotion = $.humanoidAnimation("");

$.onInteract(() => { 
  $.log("try play audio...");
  myAudio.play();
});
