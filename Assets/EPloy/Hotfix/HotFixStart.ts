//部署:npm run build

import {UnityEngine, EPloy, System} from 'csharp'
import {$ref, $unref, $generic, $promise, $typeof} from 'puerts'

class HotFixStart {

    public isEditorRes: boolean

    public Person() {
        this.isEditorRes = false
        console.log(EPloy.GameStart.Instance) 
    }
}