/* eslint no-undef: "off", no-unused-vars: "off" */
let data = {}
data.definition = 'DesignMate.gh'
data.inputs = {
  // 'Count':document.getElementById('count').valueAsNumber,
  // 'Radius':document.getElementById('radius').valueAsNumber,
  // 'Length':document.getElementById('length').valueAsNumber
}

let _threeMesh, _threeMesh1, _threeMesh2, _threeMesh3, threeMesh, threeMeshCentral
let _threeMaterialSlabs, _threeMaterialColumns, _threeMaterialCores, _threeMaterialGround, _threeMaterialFacade, _threeMaterialBeams
let rhino
var _threeMeshDict = {};
var threeMeshDict = {};
let meshObject

rhino3dm().then(async m => {
  console.log('Loaded rhino3dm.')
  rhino = m // global

  init()
  compute()
})

/**
 * Call appserver
 */
async function compute(){
  let t0 = performance.now()
  const timeComputeStart = t0

  console.log('inputs:')
  console.log(data.inputs)

  const request = {
    'method':'POST',
    'body': JSON.stringify(data),
    'headers': {'Content-Type': 'application/json'}
  }

  let headers = null

  try {
    const response = await fetch('/solve', request)

    if(!response.ok)
      throw new Error(response.statusText)
      
    headers = response.headers.get('server-timing')
    const responseJson = await response.json()

    // Request finished. Do processing here.
    let t1 = performance.now()
    const computeSolveTime = t1 - timeComputeStart
    t0 = t1

    // hide spinner
    document.getElementById('loader').style.display = 'none'
    console.log('data:')
    // console.log(responseJson.values)
    let dataIn
    var meshDict = {};

    counter = 0
    for (var dataPair of responseJson.values)
    {
      const { ParamName, InnerTree } = dataPair
      // console.log(ParamName)
      keys = Object.keys(InnerTree)
      var meshList = []
      for (var key of keys)
      {
        for (singleMesh of InnerTree[key])
        {
          dataIn = JSON.parse(singleMesh.data)
          let meshIn = rhino.DracoCompression.decompressBase64String(dataIn)
          meshList.push(meshIn)
        }
      }
      meshDict[ParamName] = meshList
      counter += 1
    }
    // console.log(meshDict)

    // let data = JSON.parse(responseJson.values[0].InnerTree['{0}'][0].data)
    // let mesh = rhino.DracoCompression.decompressBase64String(data)
    // let data1 = JSON.parse(responseJson.values[1].InnerTree['{1}'][0].data)
    // let mesh1 = rhino.DracoCompression.decompressBase64String(data1)
    // let data2 = JSON.parse(responseJson.values[2].InnerTree['{2}'][0].data)
    // let mesh2 = rhino.DracoCompression.decompressBase64String(data2)
    // let data3 = JSON.parse(responseJson.values[3].InnerTree['{3}'][0].data)
    // let mesh3 = rhino.DracoCompression.decompressBase64String(data3)
      
    t1 = performance.now()
    const decodeMeshTime = t1 - t0
    t0 = t1

    // slab material
    if (!_threeMaterialSlabs) {
      _threeMaterialSlabs = new THREE.MeshBasicMaterial({ color: 0xc0c2d1 })
    }
    // column material
    if (!_threeMaterialColumns) {
      _threeMaterialColumns = new THREE.MeshBasicMaterial({ color: 0x826c68 })
    }
    // vertical circulation material
    if (!_threeMaterialCores) {
      _threeMaterialCores = new THREE.MeshBasicMaterial({ color: 0xbadbd0 })
    }
    // ground mesh material
    if (!_threeMaterialGround) {
      _threeMaterialGround = new THREE.MeshBasicMaterial({ color: 0xe6f0e9 })
    }
    // facade material
    if (!_threeMaterialFacade) {
      _threeMaterialFacade = new THREE.MeshBasicMaterial({ color: 0xf0d07a })
    }
    // beams material
    if (!_threeMaterialBeams) {
      _threeMaterialBeams = new THREE.MeshBasicMaterial({ color: 0xf0d07a })
    }

    meshNames = Object.keys(meshDict)
    // Remove old meshes
    for (var name of meshNames)
      {
        {
          meshList = _threeMeshDict[name]
          for (var ms in meshList)
          {
            if (ms)
            {
              scene.remove(ms)
              meshList.geometry.dispose()
            }
          }
        }
      }

    // And add new ones
    for (var name of meshNames)
    {
      console.log(name)
      rhMeshList = meshDict[name]
      var threeMeshList = []
      let material = _threeMaterialSlabs
      if (name.includes("Facade")) { material = _threeMaterialFacade}
      else if (name.includes("Cores")) { material = _threeMaterialCores}
      else if (name.includes("Columns")) { material = _threeMaterialColumns}
      else if (name.includes("Beams")) { material = _threeMaterialBeams}
      
      let currentScene = scene_small0
      if (name.includes("0")) {currentScene = scene_small0}
      if (name.includes("1")) {currentScene = scene_small1}
      if (name.includes("2")) {currentScene = scene_small2}
      if (name.includes("3")) {currentScene = scene_small3}
      if (name.includes("4")) {currentScene = scene_small4}
      if (name.includes("5")) {currentScene = scene_small5}
      if (name.includes("6")) {currentScene = scene_small6}
      if (name.includes("7")) {currentScene = scene_small7}
      if (name.includes("8")) {currentScene = scene_small8}
      if (name.includes("9")) {currentScene = scene_small9}
      if (name.includes("10")) {currentScene = scene_small10}
      if (name.includes("11")) {currentScene = scene_small11}

      for (var rhMesh of rhMeshList)
      {
          threeMesh = meshToThreejs(rhMesh, material)
          
          currentScene.add(threeMesh)
          
          //console.log(threeMesh)
          if (name.includes("0"))
          {
            threeMeshCentral = meshToThreejs(rhMesh, material)
            scene.add(threeMeshCentral)
          }
          threeMeshList.push(threeMesh)
      }
      //console.log(threeMeshList)

      //replaceCurrentMesh(threeMeshList)
      threeMeshDict[name] = threeMeshList
      _threeMeshDict[name] = threeMeshList
    }
    //replaceCurrentMeshDict(threeMeshDict)
    // replaceCurrentMesh([threeMesh, threeMesh1, threeMesh2, threeMesh3])

    t1 = performance.now()
    const rebuildSceneTime = t1 - t0

    console.log(`[call compute and rebuild scene] = ${Math.round(t1-timeComputeStart)} ms`)
    console.log(`  ${Math.round(computeSolveTime)} ms: appserver request`)
    let timings = headers.split(',')
    let sum = 0
    timings.forEach(element => {
      let name = element.split(';')[0].trim()
      let time = element.split('=')[1].trim()
      sum += Number(time)
      if (name === 'network') {
        console.log(`  .. ${time} ms: appserver<->compute network latency`)
      } else {
        console.log(`  .. ${time} ms: ${name}`)
      }
    })
    console.log(`  .. ${Math.round(computeSolveTime - sum)} ms: local<->appserver network latency`)
    console.log(`  ${Math.round(decodeMeshTime)} ms: decode json to rhino3dm mesh`)
    console.log(`  ${Math.round(rebuildSceneTime)} ms: create threejs mesh and insert in scene`)

  } catch(error) {
    console.error(error)
  }
  
}

/**
 * Called when a slider value changes in the UI. Collect all of the
 * slider values and call compute to solve for a new scene
 */
function onSliderChange () {
  // show spinner
  document.getElementById('loader').style.display = 'block'

  // get slider values
  data.inputs = {
    // 'Count':document.getElementById('count').valueAsNumber,
    // 'Radius':document.getElementById('radius').valueAsNumber,
    // 'Length':document.getElementById('length').valueAsNumber
  }
  compute()
}

// BOILERPLATE //

var scene, scene_small0, scene_small1, scene_small2, scene_small3, scene_small4, scene_small5, scene_small6, scene_small7, scene_small8, scene_small9, scene_small10, scene_small11, camera, renderer, renderer_small, controls

function init () {
  scene = new THREE.Scene()
  scene.background = new THREE.Color(1,1,1)

  scene_small0 = new THREE.Scene()
  scene_small0.background = new THREE.Color(1,1,1)

  scene_small1 = new THREE.Scene()
  scene_small1.background = new THREE.Color(1,1,1)

  scene_small2 = new THREE.Scene()
  scene_small2.background = new THREE.Color(1,1,1)

  scene_small3 = new THREE.Scene()
  scene_small3.background = new THREE.Color(1,1,1)

  scene_small4 = new THREE.Scene()
  scene_small4.background = new THREE.Color(1,1,1)

  scene_small5 = new THREE.Scene()
  scene_small5.background = new THREE.Color(1,1,1)

  scene_small6 = new THREE.Scene()
  scene_small6.background = new THREE.Color(1,1,1)

  scene_small7 = new THREE.Scene()
  scene_small7.background = new THREE.Color(1,1,1)

  scene_small8 = new THREE.Scene()
  scene_small8.background = new THREE.Color(1,1,1)

  scene_small9 = new THREE.Scene()
  scene_small9.background = new THREE.Color(1,1,1)

  scene_small10 = new THREE.Scene()
  scene_small10.background = new THREE.Color(1,1,1)

  scene_small11 = new THREE.Scene()
  scene_small11.background = new THREE.Color(1,1,1)

  camera = new THREE.PerspectiveCamera( 45, window.innerWidth/window.innerHeight, 1, 1000 )

  renderer = new THREE.WebGLRenderer({antialias: true})
  renderer.setPixelRatio( window.devicePixelRatio )
  renderer.setSize( window.innerWidth, window.innerHeight )
  let canvas = document.getElementById('canvas')
  canvas.appendChild( renderer.domElement )

  renderer_small0 = new THREE.WebGLRenderer({antialias: true})
  renderer_small0.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small0 = document.getElementById('canvas_small0')
  canvas_small0.append(renderer_small0.domElement)

  renderer_small1 = new THREE.WebGLRenderer({antialias: true})
  renderer_small1.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small1 = document.getElementById('canvas_small1')
  canvas_small1.append(renderer_small1.domElement)

  renderer_small2 = new THREE.WebGLRenderer({antialias: true})
  renderer_small2.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small2 = document.getElementById('canvas_small2')
  canvas_small2.append(renderer_small2.domElement)

  renderer_small3 = new THREE.WebGLRenderer({antialias: true})
  renderer_small3.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small3 = document.getElementById('canvas_small3')
  canvas_small3.append(renderer_small3.domElement)

  renderer_small4 = new THREE.WebGLRenderer({antialias: true})
  renderer_small4.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small4 = document.getElementById('canvas_small4')
  canvas_small4.append(renderer_small4.domElement)

  renderer_small5 = new THREE.WebGLRenderer({antialias: true})
  renderer_small5.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small5 = document.getElementById('canvas_small5')
  canvas_small5.append(renderer_small5.domElement)

  renderer_small6 = new THREE.WebGLRenderer({antialias: true})
  renderer_small6.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small6 = document.getElementById('canvas_small6')
  canvas_small6.append(renderer_small6.domElement)

  renderer_small7 = new THREE.WebGLRenderer({antialias: true})
  renderer_small7.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small7 = document.getElementById('canvas_small7')
  canvas_small7.append(renderer_small7.domElement)

  renderer_small8 = new THREE.WebGLRenderer({antialias: true})
  renderer_small8.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small8 = document.getElementById('canvas_small8')
  canvas_small8.append(renderer_small8.domElement)

  renderer_small9 = new THREE.WebGLRenderer({antialias: true})
  renderer_small9.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small9 = document.getElementById('canvas_small9')
  canvas_small9.append(renderer_small9.domElement)

  renderer_small10 = new THREE.WebGLRenderer({antialias: true})
  renderer_small10.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small10 = document.getElementById('canvas_small10')
  canvas_small10.append(renderer_small10.domElement)

  renderer_small11 = new THREE.WebGLRenderer({antialias: true})
  renderer_small11.setPixelRatio( window.devicePixelRatio )
  //renderer_small.setSize( window.innerWidth, window.innerHeight )
  let canvas_small11 = document.getElementById('canvas_small11')
  canvas_small11.append(renderer_small11.domElement)

  controls = new THREE.OrbitControls( camera, renderer.domElement  )

  camera.position.x = -150
  camera.position.y = 200
  camera.position.z = -250

  window.addEventListener( 'resize', onWindowResize, false )

  animate()
}

var animate = function () {
  requestAnimationFrame( animate )
  controls.update()
  renderer.render( scene, camera )
  renderer_small0.render(scene_small0, camera)
  renderer_small1.render(scene_small1, camera)
  renderer_small2.render(scene_small2, camera)
  renderer_small3.render(scene_small3, camera)
  renderer_small4.render(scene_small4, camera)
  renderer_small5.render(scene_small5, camera)
  renderer_small6.render(scene_small6, camera)
  renderer_small7.render(scene_small7, camera)
  renderer_small8.render(scene_small8, camera)
  renderer_small9.render(scene_small9, camera)
  renderer_small10.render(scene_small10, camera)
  renderer_small11.render(scene_small11, camera)
}
  
function onWindowResize() {
  camera.aspect = window.innerWidth / window.innerHeight
  camera.updateProjectionMatrix()
  renderer.setSize( window.innerWidth, window.innerHeight )
  animate()
}

function replaceCurrentMeshDict (newMeshDict)
{
    console.log(newMeshDict)
    meshNames = Object.keys(newMeshDict)
    for (var name of meshNames)
    {
      {
        meshList = _threeMeshDict[name]
        for (var ms in meshList)
        {
          if (ms)
          {
            scene.remove(ms)
            meshList.geometry.dispose()
          }
        }
        newMeshList = newMeshDict[name]
        //console.log(newMeshList)
        for (var newMs in newMeshList)
        {
          //console.log(newMs)
          meshObject = newMs
          scene.add(meshObject)
        }
      }
      _threeMeshDict[name] = newMeshDict[name]
    }
}

function replaceCurrentMesh (meshes) {
  if (_threeMesh) {
    scene.remove(_threeMesh)
    _threeMesh.geometry.dispose()
  }
  _threeMesh = meshes[0]
  scene.add(_threeMesh)
  
  // if (_threeMesh1) {
  //   scene.remove(_threeMesh1)
  //   _threeMesh1.geometry.dispose()
  // }
  // _threeMesh1 = meshes[1]
  // scene.add(_threeMesh1)
  
  // if (_threeMesh2) {
  //   scene_small.remove(_threeMesh2)
  //   _threeMesh2.geometry.dispose()
  // }
  // _threeMesh2 = meshes[2]
  // scene_small.add(_threeMesh2)
  
  // if (_threeMesh3) {
  //   scene.remove(_threeMesh3)
  //   _threeMesh3.geometry.dispose()
  // }
  // _threeMesh3 = meshes[3]
  // scene.add(_threeMesh3)
}

function meshToThreejs (mesh, material) {
  let loader = new THREE.BufferGeometryLoader()
  var geometry = loader.parse(mesh.toThreejsJSON())
  return new THREE.Mesh(geometry, material)
}
