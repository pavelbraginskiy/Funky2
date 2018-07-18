using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Funky{
    public class StringClaimer{
        private readonly string _toClaim;
        private Stack<ClaimLoc> _prevClaims = new Stack<ClaimLoc>();

        private static readonly Regex Whitespace = new Regex(@"^(\$\*([^*]|\*[^$])*\*\$|\$[^*\r\n].*|\s)*", RegexOptions.Compiled);

        public bool Wsignored = true;
        private int _offset;
        public StringClaimer(string text){
            _toClaim = text;
        }

        public int Location(){
            return _offset;
        }

        public string SubString(int a, int b){
            return _toClaim.Substring(a, b - a);
        }

        public string SubString(int a){
            return SubString(a, _offset);
        }

        public Claim Claim(Regex method){
            if(Wsignored)
                _offset += Whitespace.Match(_toClaim.Substring(_offset)).Length;

            Match stringMatch = method.Match(_toClaim.Substring(_offset));
            if((!stringMatch.Success) || stringMatch.Index > 0){ // Incase I forget a ^ somewhere. Try not to forget a ^ somewhere, please taco.
                return new Claim();
            }
            Claim newClaim = new Claim(method, stringMatch, this);
            _prevClaims.Push(new ClaimLoc(newClaim, _offset));
            _offset += stringMatch.Length;
            return newClaim;
        }

        public Claim FailPoint(){
            if(Wsignored)
                _offset += Whitespace.Match(_toClaim.Substring(_offset)).Length;

            Claim c = new Claim(null, null, this) {Success = true};
            _prevClaims.Push(new ClaimLoc(c, _offset));
            return c;
        }

        public bool Revert(Claim claim){
            Stack<ClaimLoc> storeStack = new Stack<ClaimLoc>(_prevClaims);
            while(_prevClaims.Count > 0){
                ClaimLoc top = storeStack.Pop();
                if(top.Claim == claim){
                    _offset = top.Location;
                    return true;
                }
            }
            _prevClaims = storeStack;
            return false;
        }

        public bool PopTo(Claim claim){
            Stack<ClaimLoc> storeStack = new Stack<ClaimLoc>(_prevClaims);
            while(_prevClaims.Count > 0){
                ClaimLoc top = storeStack.Pop();
                if(top.Claim == claim){
                    return true;
                }
            }
            _prevClaims = storeStack;
            return false;
        }

    }

    public struct ClaimLoc{
        public Claim Claim;
        public int Location;
        public ClaimLoc(Claim claim, int location){
            Claim = claim;
            Location = location;
        }
    }

    public class Claim{
        public bool Success;
        private readonly Regex _claimMethod;
        private readonly Match _match;
        private readonly StringClaimer _claimer;

        public Claim(Regex claimMethod, Match match, StringClaimer claimer){
            Success = true;
            _claimMethod = claimMethod;
            _match = match;
            _claimer = claimer;
        }

        public Claim(){}

        /// <exception cref="Funky.FailedClaimException">Throws a Failed Claim Exception if the claim didn't succeed.</exception>
        public string GetText(){
            if(!Success)
                throw new FailedClaimException();
            return _match.Value;
        }

                /// <exception cref="Funky.FailedClaimException">Throws a Failed Claim Exception if the claim didn't succeed.</exception>
        public Match GetMatch(){
            if(!Success)
                throw new FailedClaimException();
            return _match;
        }

        /// <exception cref="Funky.FailedClaimException">Throws a Failed Claim Exception if the claim didn't succeed.</exception>
        public Regex GetMethod(){
            if(!Success)
                throw new FailedClaimException();
            return _claimMethod;
        }

        /// <exception cref="Funky.FailedClaimException">Throws a Failed Claim Exception if the claim didn't succeed.</exception>
        public bool Pass(){
            if(!Success)
                throw new FailedClaimException();
            return _claimer.PopTo(this);
        }

        /// <exception cref="Funky.FailedClaimException">Throws a Failed Claim Exception if the claim didn't succeed.</exception>
        public bool Fail(){
            if(!Success)
                throw new FailedClaimException();
            return _claimer.Revert(this);
        }
    }

    public class FailedClaimException : Exception
    {
        public FailedClaimException() { }
        public FailedClaimException(string message) : base(message) { }
        public FailedClaimException(string message, Exception inner) : base(message, inner) { }
        protected FailedClaimException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}