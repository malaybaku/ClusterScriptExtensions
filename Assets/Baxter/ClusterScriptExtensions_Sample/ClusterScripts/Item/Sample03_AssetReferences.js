// AudioClipなど、アセットやprefabへの参照をカスタムできるようにするサンプル。

// @field(AudioClip)
const myAudio = $.audio("");

// @field(HumanoidAnimation)
const myMotion = $.humanoidAnimation("");

// @field(WorldItem)
const myOtherItem = $.worldItemReference("");

// @field(WorldItemTemplate)
const myItemTemplate = new WorldItemTemplateId("");

// @field(Material)
const myMaterial = $.material("");

$.onInteract((player) => { 

  // AudioClip: 再生する
  myAudio.play();

  // HumanoidAnimation: 1sec時点でのポーズを適用
  let sampleTime = myMotion.getLength() > 1.0 ? 1.0 : myMotion.getLength();
  let pose = myMotion.getSample(sampleTime);
  player.setHumanoidPose(pose, {
    timeoutSeconds: 3.0,
    timeoutTransitionSeconds: 0.2,
    transitionSeconds: 0.2
  });

  // WorldItem: 鉛直真上に跳ね上げる
  myOtherItem.addImpulsiveForce(new Vector3(0, 3, 0));

  // WorldItemTemplate: 特定の位置にスポーンさせる
  let spawnPosition = $.subNode("ItemSpawnPoint").getGlobalPosition();
  $.createItem(myItemTemplate, spawnPosition, new Quaternion().identity());

  // Material: ランダムな色合いに変更
  let r = Math.random();  
  let g = Math.random();
  let b = Math.random();
  myMaterial.setBaseColor(r, g, b, 1.0);
});
