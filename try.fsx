open System
open System.Drawing

type BehaviorContext = { Time: float32 }
type Behavior<'T> =
    | BehaviorFunc of (BehaviorContext -> 'T)
let sample(a) = BehaviorFunc(a)
let forever(n) = sample(fun _ -> n)
let time = sample(fun t -> t.Time)
let wiggle = sample(fun t -> sin(t.Time * float32(Math.PI)))

type System.Single with
    member x.forever = forever(x)

let readvalue time (BehaviorFunc bfunc) =
    bfunc {Time = time}

12.0f.forever |> readvalue 1.5f
wiggle |> readvalue 1.5f
time |> readvalue 1.5f

module Behavior =
    let map f (BehaviorFunc bfunc) =
        sample(bfunc >> f)
    let lift1 f behavior = map f behavior
    let lift2 f (BehaviorFunc bf1) (BehaviorFunc bf2) =
        sample ( fun t -> f (bf1(t)) (bf2(t)) )
    let lift3 f (BehaviorFunc bf1) (BehaviorFunc bf2) (BehaviorFunc bf3) =
        sample (fun t -> f (bf1(t)) (bf2(t)) (bf3(t)) )


type Drawing =
    abstract Draw: Graphics -> unit
let drawing(f) =
    { new Drawing with member x.Draw(gr) = f(gr) }

module Drawings =
    let circle brush size =
        drawing ( fun (g: Graphics) ->
            g.FillEllipse(brush, -size/2.0f, -size/2.0f, size, size)
            )

    let translate x y (img: Drawing) =
        drawing( fun (g: Graphics) ->
            g.TranslateTransform(x, y)
            img.Draw(g);
            g.TranslateTransform(-x, -y)
        )
    
    let compose (img1: Drawing) (img2: Drawing) =
        drawing( fun (g: Graphics) ->
            img1.Draw(g)
            img2.Draw(g)
        )

open Drawings
let greenCircle = circle Brushes.OliveDrab 150.0f
let blueCircle = circle Brushes.SteelBlue 150.0f

let greenAndBlue = 
    compose (translate 200.0f 200.0f greenCircle) (translate 250.0f 250.0f blueCircle)

let drawImage (width: int, height: int) space (image: Drawing) =
    let bmp = new Bitmap(width, height)
    use gr = Graphics.FromImage(bmp)
    gr.Clear(Color.White)
    gr.TranslateTransform(space, space)
    image.Draw(gr)
    bmp
let docImage = drawImage(500,500) 20.0f greenAndBlue

open System.Windows.Forms

let main = new Form(Text = "Document", BackgroundImage = docImage, Width = docImage.Width, Height = docImage.Height)
ignore (main.ShowDialog())

type AnimationForm() as this =
    inherit Form()
    let emptyAnim = forever(drawing(ignore))
    let mutable startTime = DateTime.UtcNow
    let mutable anim = emptyAnim
    do 
        this.SetStyle(ControlStyles.AllPaintingInWmPaint |||
                      ControlStyles.OptimizedDoubleBuffer, true)
        let tmr = new Timers.Timer(Interval = 25.0)
        tmr.Elapsed.Add(fun _ -> this.Invalidate())
        tmr.Start()

    member x.Animation
        with get() = anim
        and set(newAnim) =
            anim <- newAnim
            startTime <- DateTime.UtcNow

    override x.OnPaint(e) =
        let w, h = x.ClientSize.Width, x.ClientSize.Height
        e.Graphics.Clear(Color.White)
        e.Graphics.TranslateTransform(float32(w)/2.0f, float32(h)/2.0f)
        let elapsed = (DateTime.UtcNow - startTime).TotalSeconds
        let currentDrawing = anim |> readvalue (float32(elapsed))
        currentDrawing.Draw(e.Graphics)

let translate x y img = Behavior.lift3 Drawings.translate x y img
let wiggle200 = Behavior.lift2 (*) wiggle 200.0f.forever
let af = new AnimationForm(ClientSize = Size(1100, 1100), Visible = true)
af.Animation <- translate wiggle200 0.0f.forever (forever greenAndBlue)

let circle brush size =
    Behavior.lift2 Drawings.circle brush size
let (--) anim1 anim2 =
    Behavior.lift2 Drawings.compose anim1 anim2

type Behavior<'T> with
    static member (+) (a: Behavior<float32>, b) =
        Behavior.lift2 (+) a b
    static member (*) (a: Behavior<float32>, b) =
        Behavior.lift2 (*) a b

let wait shift (BehaviorFunc bfunc) =
    sample(fun t -> bfunc { t with Time = t.Time + shift})
let faster q (BehaviorFunc bfunc) =
    sample(fun t -> bfunc { t with Time = t.Time * q}) 

let rotate (dist:float32) speed img =
    let pos = Behavior.lift2 (*) wiggle dist.forever
    // let pos = wiggle * dist.forever
    img |> translate pos (wait 0.5f pos) |> faster speed

let sun = circle (forever Brushes.Goldenrod) 100.0f.forever
let earth = circle (forever Brushes.SteelBlue) 50.0f.forever
let moon = circle (forever Brushes.DimGray) 20.0f.forever

let planets = 
    sun -- 
        (earth -- (moon |> rotate 40.0f 12.0f)
            |> rotate 160.0f 1.3f)
    |> faster 0.1f            

let afpl = new AnimationForm(ClientSize = Size(500, 500), Visible = true)
afpl.Animation <- planets






