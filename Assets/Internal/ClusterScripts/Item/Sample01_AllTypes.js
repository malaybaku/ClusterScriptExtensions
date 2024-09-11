// @field(bool)
const myBool = false;
// @field(int)
const myInt = 10;
// @field(float)
const myFloat = 2.3;
// @field(string)
const myString = "Test";
// @field(Vector2)
const myVector2 = new Vector2(1, 2);
// @field(Vector3)
const myVector3 = new Vector3(3, 4, 5);
// @field(Quaternion)
const myQuaternion = new Quaternion();

$.onInteract(() => { 
  $.log(`myBool = ${myBool}`);
  $.log(`myInt = ${myInt}`);
  $.log(`myFloat = ${myFloat}`);
  $.log(`myString = ${myString}`);
  $.log(`myVector2 = ${myVector2}`);
  $.log(`myVector3 = ${myVector3}`);
  $.log(`myQuaternion = ${myQuaternion}`);
});
