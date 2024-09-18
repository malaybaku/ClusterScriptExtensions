// @field(auto)
const myBool = false;

// NOTE: これは意図はintかもしれないがfloatになる (intにしたい場合、明示的にintを指定しないとダメ)
// @field(auto)
const myInt = 10;
// NOTE: これもfloat扱いになる
// @field(auto), @range(-10, 10)
const myRangeInt = 0;
// @field(auto)
const myFloat = 2.3;
// @field(auto), @range(-1.5, 1.5)
const myRangeFloat = 0.0;
// @field(auto)
const myString = "Test";
// @field(auto), @textArea
const myLongString = "Test Long Text";
// @field(auto)
const myVector2 = new Vector2(1, 2);
// @field(auto)
const myVector3 = new Vector3(3, 4, 5);
// @field(auto)
const myQuaternion = new Quaternion();

// @field(auto)
const myAudio = $.audio("");

// @field(auto)
const myMotion = $.humanoidAnimation("");

// @field(auto)
const myOtherItem = $.worldItemReference("");

// @field(auto)
const myItemTemplate = new WorldItemTemplateId("");

// @field(auto)
const myMaterial = $.material("");
