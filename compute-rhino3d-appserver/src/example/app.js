/* eslint no-undef: "off", no-unused-vars: "off" */
let data = {}
data.definition = 'DesignMate.gh'
data.inputs = {
  'Count':document.getElementById('count').valueAsNumber,
  'Radius':document.getElementById('radius').valueAsNumber,
  'Length':document.getElementById('length').valueAsNumber
}

let _threeMesh, _threeMesh1, _threeMesh2, _threeMesh3, _threeMaterial, _threeMaterial1, _threeMaterial2, _threeMaterial3, rhino

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
    console.log(responseJson.values)
    let data = JSON.parse(responseJson.values[0].InnerTree['{0}'][0].data)
    let mesh = rhino.DracoCompression.decompressBase64String(data)
    let data1 = JSON.parse(responseJson.values[1].InnerTree['{0}'][0].data)
    let mesh1 = rhino.DracoCompression.decompressBase64String(data1)
    let data2 = JSON.parse(responseJson.values[2].InnerTree['{0}'][0].data)
    let mesh2 = rhino.DracoCompression.decompressBase64String(data2)
    let data3 = JSON.parse(responseJson.values[3].InnerTree['{0}'][0].data)
    let mesh3 = rhino.DracoCompression.decompressBase64String(data3)
      
    t1 = performance.now()
    const decodeMeshTime = t1 - t0
    t0 = t1

    // slab material
    if (!_threeMaterial) {
      _threeMaterial = new THREE.MeshBasicMaterial({ color: 0xc0c2d1 })
    }
    // column material
    if (!_threeMaterial1) {
      _threeMaterial1 = new THREE.MeshBasicMaterial({ color: 0x826c68 })
    }
    // vertical circulation material
    if (!_threeMaterial2) {
      _threeMaterial2 = new THREE.MeshBasicMaterial({ color: 0xbadbd0 })
    }
    // ground mesh material
    if (!_threeMaterial3) {
      _threeMaterial3 = new THREE.MeshBasicMaterial({ color: 0xe6f0e9 })
    }
    let threeMesh = meshToThreejs(mesh, _threeMaterial)
    mesh.delete()

    let threeMesh1 = meshToThreejs(mesh1, _threeMaterial1)
    mesh1.delete()

    let threeMesh2 = meshToThreejs(mesh2, _threeMaterial2)
    mesh2.delete()

    let threeMesh3 = meshToThreejs(mesh3, _threeMaterial3)
    mesh3.delete()
    replaceCurrentMesh([threeMesh, threeMesh1, threeMesh2, threeMesh3])

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
    'Count':document.getElementById('count').valueAsNumber,
    'Radius':document.getElementById('radius').valueAsNumber,
    'Length':document.getElementById('length').valueAsNumber
  }
  compute()
}

// BOILERPLATE //

var scene, camera, renderer, controls

function init () {
  scene = new THREE.Scene()
  scene.background = new THREE.Color(1,1,1)
  camera = new THREE.PerspectiveCamera( 45, window.innerWidth/window.innerHeight, 1, 1000 )

  renderer = new THREE.WebGLRenderer({antialias: true})
  renderer.setPixelRatio( window.devicePixelRatio )
  renderer.setSize( window.innerWidth, window.innerHeight )
  let canvas = document.getElementById('canvas')
  canvas.appendChild( renderer.domElement )

  controls = new THREE.OrbitControls( camera, renderer.domElement  )

  camera.position.x = -250
  camera.position.y = 150
  camera.position.z = -250

  window.addEventListener( 'resize', onWindowResize, false )

  animate()
}

var animate = function () {
  requestAnimationFrame( animate )
  controls.update()
  renderer.render( scene, camera )
}
  
function onWindowResize() {
  camera.aspect = window.innerWidth / window.innerHeight
  camera.updateProjectionMatrix()
  renderer.setSize( window.innerWidth, window.innerHeight )
  animate()
}

function replaceCurrentMesh (meshes) {
  if (_threeMesh) {
    scene.remove(_threeMesh)
    _threeMesh.geometry.dispose()
  }
  _threeMesh = meshes[0]
  scene.add(_threeMesh)
  
  if (_threeMesh1) {
    scene.remove(_threeMesh1)
    _threeMesh1.geometry.dispose()
  }
  _threeMesh1 = meshes[1]
  scene.add(_threeMesh1)
  
  if (_threeMesh2) {
    scene.remove(_threeMesh2)
    _threeMesh2.geometry.dispose()
  }
  _threeMesh2 = meshes[2]
  scene.add(_threeMesh2)
  
  if (_threeMesh3) {
    scene.remove(_threeMesh3)
    _threeMesh3.geometry.dispose()
  }
  _threeMesh3 = meshes[3]
  scene.add(_threeMesh3)
}

function meshToThreejs (mesh, material) {
  let loader = new THREE.BufferGeometryLoader()
  var geometry = loader.parse(mesh.toThreejsJSON())
  return new THREE.Mesh(geometry, material)
}
