// 数値や文字列など、定数値で表現できるデータをカスタムできるようにするサンプル。

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
  showLog(
    `myBool = ${myBool}\n` + 
    `myInt = ${myInt}\n` +
    `myFloat = ${myFloat}\n` + 
    `myString = ${myString}\n` + 
    `myVector2 = ${myVector2}\n` + 
    `myVector3 = ${myVector3}\n` + 
    `myQuaternion = ${myQuaternion}`
  );
});


const showLog = (text) => { 
  $.subNode("LogText").setText(text);
  $.log(text);
}
