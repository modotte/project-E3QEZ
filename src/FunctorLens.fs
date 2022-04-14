module FunctorLens

open FSharpPlus.Lens

[<AbstractClass>]
type Functor<'a>() =
    abstract member Select<'b> : ('a -> 'b) -> Functor<'b>
    static member Map(x: Functor<'a>, f: 'a -> 'b) : Functor<'b> = x.Select(f)

type IdentityFunctor<'a>(value: 'a) =
    inherit Functor<'a>()
    member __.Run = value
    override __.Select<'b>(f: 'a -> 'b) = IdentityFunctor(f value) :> Functor<'b>

type ConstFunctor<'p, 'a>(value: 'p) =
    inherit Functor<'a>()
    member __.Run = value
    override __.Select<'b>(f: 'a -> 'b) = ConstFunctor(value) :> Functor<'b>

let setl' optic value (source: 's) : 't =
    let (x: Functor<'t>) = optic (fun _ -> IdentityFunctor value :> Functor<'v>) source
    (x :?> IdentityFunctor<'t>).Run

let view' optic (source: 's) : 'a =
    let (x: Functor<'t>) = optic (fun x -> ConstFunctor x :> Functor<'b>) source
    (x :?> ConstFunctor<'a, 't>).Run

let (^..) f x = view' x f
let (.->>) f x = setl' f x
