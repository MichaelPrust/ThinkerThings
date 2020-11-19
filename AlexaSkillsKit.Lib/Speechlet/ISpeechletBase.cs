// Copyright 2018 Stefan Negritoiu (FreeBusy) and contributors. See LICENSE file for more information.

using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;
using System;

namespace AlexaSkillsKit.Speechlet
{
    public interface ISpeechletBase
    {
        bool OnRequestValidation(SpeechletRequestValidationResult result, DateTime referenceTimeUtc, SpeechletRequestEnvelope requestEnvelope);
    }
}